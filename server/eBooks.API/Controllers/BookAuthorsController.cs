using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("[controller]")]
    public class BookAuthorsController : ControllerBase
    {
        protected IBookAuthorsService _service;
        protected AccessControlHandler _accessControlHandler;

        public BookAuthorsController(IBookAuthorsService service, AccessControlHandler accessControlHandler)
        {
            _service = service;
            _accessControlHandler = accessControlHandler;
        }

        [HttpGet("{bookId}")]
        public async Task<List<BookAuthorsRes>> GetByBookId(int bookId)
        {
            return await _service.GetByBookId(bookId);
        }

        [HttpPost("{bookId}")]
        public async Task<List<BookAuthorsRes>> Post(int bookId, BookAuthorsReq req)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(bookId);
            return await _service.Post(bookId, req);
        }

        [HttpPatch("{bookId}")]
        public async Task<List<BookAuthorsRes>> Patch(int bookId, BookAuthorsReq req)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(bookId);
            return await _service.Patch(bookId, req);
        }

        [HttpDelete("{bookId}/{authorId}")]
        public async Task<BookAuthorsRes> Delete(int bookId, int authorId)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(bookId);
            return await _service.Delete(bookId, authorId);
        }
    }
}
