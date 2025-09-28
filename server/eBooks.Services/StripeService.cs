using eBooks.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using eBooks.Interfaces;
using Stripe;
using Stripe.Checkout;
using eBooks.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using eBooks.Database.Models;
using Microsoft.Extensions.Logging;
using eBooks.Models.Responses;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using EasyNetQ;
using eBooks.Models.Messages;
using MapsterMapper;

namespace eBooks.Services
{
    public class StripeService : IStripeService
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IBus _bus;
        protected IConfiguration _config;
        protected ILogger<StripeService> _logger;

        public StripeService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBus bus, ILogger<StripeService> logger, IConfiguration config)
        {
            _db = db;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _bus = bus;
            _logger = logger;
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public async Task<StripeRes> GetStripeAccountLink()
        {
            var user = await _db.Set<User>().FindAsync(GetUserId());
            if (user == null)
                throw new ExceptionNotFound();
            var accountService = new AccountService();
            var account = await accountService.GetAsync(user.StripeAccountId);
            if (!account.DetailsSubmitted)
            {
                var accountLinkService = new AccountLinkService();
                var accountLink = await accountLinkService.CreateAsync(new AccountLinkCreateOptions
                {
                    Account = user.StripeAccountId,
                    RefreshUrl = "ebooks://profile",
                    ReturnUrl = "ebooks://profile",
                    Type = "account_onboarding"
                });
                return new StripeRes { Url = accountLink.Url };
            }
            else
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://api.stripe.com/v1/");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["Stripe:SecretKey"]);
                var response = await httpClient.PostAsync($"accounts/{user.StripeAccountId}/login_links", null);
                var content = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(content);
                var loginUrl = document.RootElement.GetProperty("url").GetString();
                return new StripeRes { Url = loginUrl };
            }
        }

        public async Task<StripeRes> CreateCheckoutSession(int bookId)
        {
            var errors = new Dictionary<string, List<string>>();
            var userId = GetUserId();
            var user = await _db.Set<User>().FindAsync(userId);
            var book = await _db.Books.Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == bookId);
            if (book == null)
                throw new ExceptionNotFound();
            if (book.DeletionReason != null)
                throw new ExceptionBadRequest("This book is deleted");
            if (await _db.Set<AccessRight>().AnyAsync(x => x.UserId == userId && x.BookId == bookId))
                errors.AddError("Book", "You already possess this book");
            if (book.Price == 0)
                errors.AddError("Book", "This book is free, you cannot buy it");
            if (userId == book.PublisherId)
                errors.AddError("Book", "You cannot add your own book");
            if (book.StateMachine != "approve")
                errors.AddError("Book", "This book is not active right now");
            if (!user.IsEmailVerified)
                errors.AddError("Email", "You must verify your email before buying a book");
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            var finalPrice = Helpers.CalculateDiscountedPrice((decimal)book.Price, book.DiscountPercentage, book.DiscountStart, book.DiscountEnd);
            var priceInCents = (long)(finalPrice * 100);
            var platformFee = (long)(finalPrice * 100 * 0.10m);
            var options = new SessionCreateOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "bookId", book.BookId.ToString() },
                    { "userId", userId.ToString() },
                    { "publisherId", book.PublisherId.ToString() },
                    { "totalPrice", finalPrice.Value.ToString("F2", CultureInfo.InvariantCulture) }
                },
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "eur",
                            UnitAmount = priceInCents,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = book.Title,
                                Description = book.Description,
                                Images = new List<string> { $"{_config["AppSettings:ngrokURL"]}/images/{book.FilePath}.webp" }
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = "ebooks://profile",
                CancelUrl = "ebooks://profile",
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    ApplicationFeeAmount = platformFee,
                    TransferData = new SessionPaymentIntentDataTransferDataOptions
                    {
                        Destination = book.Publisher.StripeAccountId
                    }
                }
            };
            SessionService service;
            Session session;
            try
            {
                service = new SessionService();
                session = await service.CreateAsync(options);
            }
            catch
            {
                throw new ExceptionBadRequest("You must fully complete your stripe account");
            }
            return new StripeRes
            {
                Url = session.Url
            };
        }

        public async Task HandleStripeWebhook(string json, string stripeSignature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _config["Stripe:WebhookSecret"]);
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session?.Metadata == null)
                        return;
                    if (!session.Metadata.TryGetValue("bookId", out var bookIdStr) ||
                        !session.Metadata.TryGetValue("userId", out var userIdStr) ||
                        !session.Metadata.TryGetValue("publisherId", out var publisherIdStr) ||
                        !session.Metadata.TryGetValue("totalPrice", out var totalPriceStr) ||
                        !int.TryParse(bookIdStr, out var bookId) ||
                        !int.TryParse(userIdStr, out var userId) ||
                        !int.TryParse(publisherIdStr, out var publisherId) ||
                        !decimal.TryParse(totalPriceStr, out var totalPrice)
                    )
                        return;
                    var book = await _db.Books.FirstOrDefaultAsync(x => x.BookId == bookId);
                    var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
                    var publisher = await _db.Users.FirstOrDefaultAsync(x => x.UserId == publisherId);
                    if (book == null || user == null || publisher == null)
                        return;
                    var paymentIntentId = session.PaymentIntentId;
                    var paymentStatus = session.PaymentStatus;
                    var paymentIntentServiceSuccess = new PaymentIntentService();
                    var paymentIntentSuccess = await paymentIntentServiceSuccess.GetAsync(paymentIntentId);
                    totalPrice /= 100;
                    var purchase = new Purchase
                    {
                        UserId = userId,
                        PublisherId = publisherId,
                        BookId = bookId,
                        TotalPrice = totalPrice,
                        PaymentStatus = "success",
                        PaymentMethod = "card",
                        TransactionId = paymentIntentId
                    };
                    var accessRight = new AccessRight
                    {
                        UserId = userId,
                        BookId = bookId
                    };
                    _db.Set<Purchase>().Add(purchase);
                    _db.Set<AccessRight>().Add(accessRight);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"Payment successfull userId:{userId} bookId:{bookId} totalPrice:{totalPrice}");
                    _bus.PubSub.Publish(new PaymentCompleted { Purchase = _mapper.Map<PurchasesRes>(purchase) });
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent == null)
                        return;
                    if (paymentIntent.Metadata == null)
                        return;
                    if (!paymentIntent.Metadata.TryGetValue("bookId", out var bookIdStr) ||
                        !paymentIntent.Metadata.TryGetValue("userId", out var userIdStr) ||
                        !paymentIntent.Metadata.TryGetValue("publisherId", out var publisherIdStr) ||
                        !paymentIntent.Metadata.TryGetValue("totalPrice", out var totalPriceStr) ||
                        !int.TryParse(bookIdStr, out var bookId) ||
                        !int.TryParse(userIdStr, out var userId) ||
                        !int.TryParse(publisherIdStr, out var publisherId) ||
                        !decimal.TryParse(totalPriceStr, out var totalPrice)
                    )
                        return;
                    var book = await _db.Books.FirstOrDefaultAsync(x => x.BookId == bookId);
                    var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
                    var publisher = await _db.Users.FirstOrDefaultAsync(x => x.UserId == publisherId);
                    if (book == null || user == null || publisher == null)
                        return;
                    totalPrice /= 100;
                    var purchase = new Purchase
                    {
                        UserId = userId,
                        PublisherId = publisherId,
                        BookId = bookId,
                        TotalPrice = totalPrice,
                        PaymentStatus = "failed",
                        PaymentMethod = "card",
                        TransactionId = paymentIntent.Id
                    };
                    _db.Set<Purchase>().Add(purchase);
                    await _db.SaveChangesAsync();
                    _logger.LogError($"Payment failed userId:{userId} bookId:{bookId} totalPrice:{totalPrice}");
                    _bus.PubSub.Publish(new PaymentCompleted { Purchase = _mapper.Map<PurchasesRes>(purchase) });
                }
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
