using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("[controller]")]
    public class BaseUserContextController<TSearch, TRequest, TResponse> : ControllerBase
    {
        protected IBaseUserContextService<TSearch, TRequest, TResponse> _service;

        public BaseUserContextController(IBaseUserContextService<TSearch, TRequest, TResponse> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<PagedResult<TResponse>> GetPaged([FromQuery] TSearch search)
        {
            return await _service.GetPaged(search);
        }

        [HttpGet("{bookId}")]
        public virtual async Task<TResponse> GetByBookId(int bookId)
        {
            return await _service.GetByBookId(bookId);
        }

        [HttpPost("{bookId}")]
        public virtual async Task<TResponse> Post(int bookId, TRequest req)
        {
            return await _service.Post(bookId, req);
        }

        [HttpPatch("{bookId}")]
        public virtual async Task<TResponse> Patch(int bookId)
        {
            return await _service.Patch(bookId);
        }

        [HttpDelete("{bookId}")]
        public virtual async Task<TResponse> Delete(int bookId)
        {
            return await _service.Delete(bookId);
        }
    }
}
