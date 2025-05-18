using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : BaseUserContextController<ReviewsReq, ReviewsRes>
    {
        public ReviewController(IReviewService service)
            : base(service)
        {
        }
    }
}
