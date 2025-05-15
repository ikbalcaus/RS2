using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IPaymentService
    {
        Task<StripeRes> CreateCheckoutSession(int bookId);
        Task HandleStripeWebhook(string json, string stripeSignature);
    }
}
