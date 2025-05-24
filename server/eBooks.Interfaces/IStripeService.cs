using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IStripeService
    {
        Task<StripeRes> GetStripeAccountLink(int userId);
        Task<StripeRes> CreateCheckoutSession(int bookId);
        Task HandleStripeWebhook(string json, string stripeSignature);
    }
}
