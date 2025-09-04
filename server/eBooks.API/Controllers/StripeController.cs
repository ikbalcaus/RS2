using Microsoft.AspNetCore.Mvc;
using eBooks.Interfaces;
using Microsoft.AspNetCore.Authorization;
using eBooks.Models.Exceptions;
using eBooks.Models.Responses;
using eBooks.API.Auth;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StripeController : ControllerBase
    {
        protected IStripeService _service;
        protected AccessControlHandler _accessControlHandler;

        public StripeController(IStripeService service, AccessControlHandler accessControlHandler)
        {
            _service = service;
            _accessControlHandler = accessControlHandler;
        }

        [Authorize(Policy = "User")]
        [HttpGet("stripe-account-link")]
        public async Task<StripeRes> GetStripeAccountLink()
        {
            return await _service.GetStripeAccountLink();
        }

        [Authorize(Policy = "User")]
        [HttpGet("{bookId}/checkout-session")]
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
