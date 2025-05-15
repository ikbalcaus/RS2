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

namespace eBooks.Services
{
    public class PaymentService : IPaymentService
    {
        protected ILogger<PaymentService> _logger;
        private readonly EBooksContext _db;
        private readonly IConfiguration _config;
        protected IHttpContextAccessor _httpContextAccessor;

        public PaymentService(EBooksContext db, IConfiguration config, IHttpContextAccessor httpContextAccessor, ILogger<PaymentService> logger)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<StripeRes> CreateCheckoutSession(int bookId)
        {
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            var book = await _db.Books.Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == bookId);
            if (book == null || string.IsNullOrWhiteSpace(book.Publisher?.StripeAccountId))
                throw new ExceptionNotFound();
            var priceInCents = (long)(book.Price * 100);
            var platformFee = (long)(book.Price * 100 * 0.10m);
            var options = new SessionCreateOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "bookId", book.BookId.ToString() },
                    { "userId", userId.ToString() },
                    { "publisherId", book.PublisherId.ToString() },
                    { "totalPrice", book.Price.ToString() }
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
                                Name = book.Title
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
                    _logger.LogInformation($"Payment failed userId:{userId} bookId:{bookId} totalPrice:{totalPrice}");
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
