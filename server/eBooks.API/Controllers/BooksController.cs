using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Books;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Publisher")]
        public override BooksRes Create(BooksCreateReq req)
        {
            return base.Create(req);
        }

        [Authorize(Roles = "Publisher")]
        public override BooksRes Update(int id, BooksUpdateReq req)
        {
            return base.Update(id, req);
        }

        [Authorize(Roles = "Publisher")]
        public override BooksRes Delete(int id)
        {
            return base.Delete(id);
        }

        [Authorize(Roles = "Publisher")]
        [HttpPatch("{id}/await")]
        public BooksRes Await(int id)
        {
            return _service.Await(id);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPatch("{id}/approve")]
        public BooksRes Approve(int id)
        {
            return _service.Approve(id);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPatch("{id}/reject")]
        public BooksRes Reject(int id, string message)
        {
            return _service.Reject(id, message);
        }

        [Authorize(Roles = "Publisher")]
        [HttpPatch("{id}/hide")]
        public BooksRes Archive(int id)
        {
            return _service.Hide(id);
        }

        [AllowAnonymous]
        [HttpGet("{id}/allowed-actions")]
        public List<string> AllowedActions(int id)
        {
            return _service.AllowedActions(id);
        }
    }
}
