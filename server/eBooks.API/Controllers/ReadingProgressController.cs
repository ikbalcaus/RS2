using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReadingProgressController : BaseUserContextController<BaseSearch, object, ReadingProgressRes>
    {
        public ReadingProgressController(IReadingProgressService service)
            : base(service)
        {
        }
    }
}
