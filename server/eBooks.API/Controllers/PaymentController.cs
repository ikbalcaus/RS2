using Microsoft.AspNetCore.Mvc;
using eBooks.Interfaces;
using Microsoft.AspNetCore.Authorization;
using eBooks.Services;
using Stripe;
using eBooks.Models.Exceptions;
using eBooks.Models.Payments;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        protected readonly IPaymentService _service;

        public PaymentsController(IPaymentService service)
        {
            _service = service;
        }

        [Authorize(Policy = "User")]
        [HttpPost("{bookId}")]
        public async Task<StripeRes> CreateCheckoutSession(int bookId)
        {
            return await _service.CreateCheckoutSession(bookId);
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task StripeWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];
            if (string.IsNullOrEmpty(signature))
                throw new ExceptionBadRequest("Missing stripe header");
            await _service.HandleStripeWebhook(json, signature);
        }
    }
}
