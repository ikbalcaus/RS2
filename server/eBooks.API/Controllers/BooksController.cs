using eBooks.Interfaces;
using eBooks.Models.Books;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : BaseController<BooksSearch, BooksInsertReq, BooksUpdateReq, BooksRes>
    {
        public BooksController(IBooksService service) : base(service)
        {
        }
    }
}
