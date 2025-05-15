using eBooks.Interfaces;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : BaseUserContextController<ReviewsRes>
    {
        public ReviewController(IReviewService service)
            : base(service)
        {
        }
    }
}
