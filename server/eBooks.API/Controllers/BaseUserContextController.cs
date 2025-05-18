using eBooks.Interfaces;
using eBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("[controller]")]
    public class BaseUserContextController<TRequest, TResponse> : ControllerBase
    {
        protected IBaseUserContextService<TRequest, TResponse> _service;

        public BaseUserContextController(IBaseUserContextService<TRequest, TResponse> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<PagedResult<TResponse>> GetPaged()
        {
            return await _service.GetPaged();
        }

        [HttpGet("{bookId}")]
        public async Task<TResponse> GetByBookId(int bookId)
        {
            return await _service.GetByBookId(bookId);
        }

        [HttpPost("{bookId}")]
        public async Task<TResponse> Post(int bookId, TRequest req)
        {
            return await _service.Post(bookId, req);
        }

        [HttpPatch("{bookId}")]
        public async Task<TResponse> Patch(int bookId)
        {
            return await _service.Patch(bookId);
        }

        [HttpDelete("{bookId}")]
        public async Task<TResponse> Delete(int bookId)
        {
            return await _service.Delete(bookId);
        }
    }
}
