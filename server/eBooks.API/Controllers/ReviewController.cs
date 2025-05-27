using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : BaseUserContextController<ReviewsPostReq, ReviewsRes>
    {
        protected new IReviewService _service;

        public ReviewController(IReviewService service)
            : base(service)
        {
            _service = service;
        }

        [Authorize(Policy = "User")]
        [HttpPut("{bookId}")]
        public async Task<ReviewsRes> Put(int bookId, ReviewsPutReq req)
        {
            return await _service.Put(bookId, req);
        }

        [Authorize(Policy = "Moderator")]
        [HttpDelete("{userId}/{bookId}")]
        public async Task<ReviewsRes> DeleteByUserAndBookId(int userId, int bookId)
        {
            return await _service.DeleteByUserAndBookId(userId, bookId);
        }
    }
}
