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
using Org.BouncyCastle.Ocsp;

namespace eBooks.Services
{
    public class StripeService : IStripeService
    {
        protected ILogger<StripeService> _logger;
        protected EBooksContext _db;
        protected IHttpContextAccessor _httpContextAccessor;
        protected EmailService _emailService;
        protected IConfiguration _config;

        public StripeService(EBooksContext db, IHttpContextAccessor httpContextAccessor, EmailService emailService, IConfiguration config, ILogger<StripeService> logger)
        {
            _logger = logger;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<StripeRes> GetStripeAccountLink(int userId)
        {
            var user = await _db.Set<User>().FindAsync(userId);
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
                    RefreshUrl = "https://example.com/refresh",
                    ReturnUrl = "https://example.com/return",
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
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : 0;
            var user = await _db.Set<User>().FindAsync(userId);
            if (await _db.Set<AccessRight>().AnyAsync(x => x.UserId == userId && x.BookId == bookId))
                throw new ExceptionBadRequest("You already possess this book");
            var book = await _db.Books.Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == bookId);
            if (book == null)
                throw new ExceptionNotFound();
            if (book.Price == 0)
                throw new ExceptionBadRequest("This book is free, you cannot buy it");
            if (userId == book.PublisherId)
                throw new ExceptionBadRequest("You cannot buy your own book");
            if (book.StateMachine != "approve")
                throw new ExceptionBadRequest("This book is not active right now");
            if (!user.IsEmailVerified)
            {
                var verificationToken = Guid.NewGuid().ToString();
                user.VerificationToken = verificationToken;
                user.TokenExpiry = DateTime.UtcNow.AddHours(24);
                await _db.SaveChangesAsync();
                await _emailService.SendEmailAsync(user.Email, "Email verification", verificationToken);
                throw new ExceptionBadRequest("Your email is not verified, please check your email and verifiy it");
            }
            var finalPrice = Helpers.CalculateDiscountedPrice(book.Price, book.DiscountPercentage, book.DiscountStart, book.DiscountEnd);
            var priceInCents = (long)(finalPrice * 100);
            var platformFee = (long)(finalPrice * 100 * 0.10m);
            var imageUrls = await _db.Set<BookImage>().Where(x => x.BookId == bookId).OrderByDescending(x => x.ModifiedAt).Select(x => _config["AppSettings:ngrokURL"] + x.ImagePath).ToListAsync();
            var options = new SessionCreateOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "bookId", book.BookId.ToString() },
                    { "userId", userId.ToString() },
                    { "publisherId", book.PublisherId.ToString() },
                    { "totalPrice", finalPrice.Value.ToString("F2", CultureInfo.InvariantCulture) }
                },
                PaymentMethodTypes = new List<string> { "card", "paypal" },
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
                                Images = imageUrls.Take(1).ToList()
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel",
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    ApplicationFeeAmount = platformFee,
                    TransferData = new SessionPaymentIntentDataTransferDataOptions
                    {
                        Destination = book.Publisher.StripeAccountId
                    }
                }
            };
            var service = new SessionService();
            var session = await service.CreateAsync(options);
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
                    var purchaseSuccess = new Purchase
                    {
                        UserId = userId,
                        PublisherId = publisherId,
                        BookId = bookId,
                        TotalPrice = totalPrice,
                        PaymentStatus = "paid",
                        PaymentMethod = "card",
                        TransactionId = paymentIntentId
                    };
                    var accessRight = new AccessRight
                    {
                        UserId = userId,
                        BookId = bookId
                    };
                    _db.Set<Purchase>().Add(purchaseSuccess);
                    _db.Set<AccessRight>().Add(accessRight);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"Payment successfull userId:{userId} bookId:{bookId} totalPrice:{totalPrice}");
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
                    var purchaseFailed = new Purchase
                    {
                        UserId = userId,
                        PublisherId = publisherId,
                        BookId = bookId,
                        TotalPrice = totalPrice,
                        PaymentStatus = "failed",
                        PaymentMethod = "card",
                        TransactionId = paymentIntent.Id
                    };
                    _db.Set<Purchase>().Add(purchaseFailed);
                    await _db.SaveChangesAsync();
                    _logger.LogError($"Payment failed userId:{userId} bookId:{bookId} totalPrice:{totalPrice}");
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
