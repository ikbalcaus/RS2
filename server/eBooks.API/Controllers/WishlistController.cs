using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WishlistController : BaseUserContextController<BaseSearch, object, WishlistRes>
    {
        public WishlistController(IWishlistService service)
            : base(service)
        {
        }
    }
}
