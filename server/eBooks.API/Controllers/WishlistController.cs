using eBooks.Interfaces;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WishlistController : BaseUserContextController<WishlistRes>
    {
        public WishlistController(IWishlistService service)
            : base(service)
        {
        }
    }
}
