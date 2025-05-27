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
    public class BookGenresController : ControllerBase
    {
        protected IBookGenresService _service;
        protected AccessControlHandler _accessControlHandler;

        public BookGenresController(IBookGenresService service, AccessControlHandler accessControlHandler)
        {
            _service = service;
            _accessControlHandler = accessControlHandler;
        }

        [HttpGet("{bookId}")]
        public async Task<List<BookGenresRes>> GetByBookId(int bookId)
        {
            return await _service.GetByBookId(bookId);
        }

        [HttpPost("{bookId}")]
        public async Task<List<BookGenresRes>> Post(int bookId, BookGenresReq req)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(bookId);
            return await _service.Post(bookId, req);
        }

        [HttpPatch("{bookId}")]
        public async Task<List<BookGenresRes>> Patch(int bookId, BookGenresReq req)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(bookId);
            return await _service.Patch(bookId, req);
        }

        [HttpDelete("{bookId}/{genreId}")]
        public async Task<BookGenresRes> Delete(int bookId, int genreId)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(bookId);
            return await _service.Delete(bookId, genreId);
        }
    }
}
