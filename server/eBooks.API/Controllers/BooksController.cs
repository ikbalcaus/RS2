using System.Dynamic;
using Azure;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : BaseController<BooksSearch, BooksCreateReq, BooksUpdateReq, BooksRes>
    {
        protected new IBooksService _service;

        public BooksController(IBooksService service) : base(service)
        {
            _service = service;
        }

        [AllowAnonymous]
        public override PagedResult<BooksRes> GetAll([FromQuery] BooksSearch search)
        {
            return base.GetAll(search);
        }

        [AllowAnonymous]
        public override BooksRes GetById(int id)
        {
            return base.GetById(id);
        }

        [HttpPatch("{id}/await")]
        public BooksRes Await(int id)
        {
            return _service.Await(id);
        }

        [HttpPatch("{id}/approve")]
        public BooksRes Approve(int id)
        {
            return _service.Approve(id);
        }

        [HttpPatch("{id}/reject")]
        public BooksRes Reject(int id, string message)
        {
            return _service.Reject(id, message);
        }

        [HttpPatch("{id}/archive")]
        public BooksRes Archive(int id)
        {
            return _service.Archive(id);
        }

        [HttpGet("{id}/allowed-actions")]
        public List<string> AllowedActions(int id)
        {
            return _service.AllowedActions(id);
        }
    }
}
