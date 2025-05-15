using eBooks.Interfaces;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoritesController : BaseUserContextController<FavoritesRes>
    {
        public FavoritesController(IFavoritesService service)
            : base(service)
        {
        }
    }
}
