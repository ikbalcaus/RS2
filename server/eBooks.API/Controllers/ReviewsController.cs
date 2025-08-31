using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewsController : BaseUserContextController<ReviewsSearch, ReviewsReq, ReviewsRes>
    {
        protected new IReviewsService _service;

        public ReviewsController(IReviewsService service)
            : base(service)
        {
            _service = service;
        }

        [AllowAnonymous]
        public override async Task<PagedResult<ReviewsRes>> GetPaged([FromQuery] ReviewsSearch search)
        {
            return await base.GetPaged(search);
        }

        [Authorize(Policy = "User")]
        [HttpPut("{bookId}")]
        public async Task<ReviewsRes> Put(int bookId, ReviewsReq req)
        {
            return await _service.Put(bookId, req);
        }

        [Authorize(Policy = "User")]
        [HttpPatch("report/{userId}/{bookId}")]
        public async Task<ReviewsRes> Report(int userId, int bookId, string reason)
        {
            return await _service.Report(userId, bookId, reason);
        }

        [Authorize(Policy = "Moderator")]
        [HttpDelete("{userId}/{bookId}")]
        public async Task<ReviewsRes> AdminDelete(int userId, int bookId)
        {
            return await _service.AdminDelete(userId, bookId);
        }
    }
}
