using eBooks.Interfaces;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReadingProgressController : BaseUserContextController<object, ReadingProgressRes>
    {
        public ReadingProgressController(IReadingProgressService service)
            : base(service)
        {
        }
    }
}
