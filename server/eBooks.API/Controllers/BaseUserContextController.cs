using eBooks.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("[controller]")]
    public class BaseUserContextController<TResponse> : ControllerBase
    {
        protected IBaseUserContextService<TResponse> _service;

        public BaseUserContextController(IBaseUserContextService<TResponse> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<List<TResponse>> GetAll()
        {
            return await _service.GetAll();
        }

        [HttpGet("{bookId}")]
        public async Task<TResponse> GetByBookId(int bookId)
        {
            return await _service.GetByBookId(bookId);
        }

        [HttpPost("{bookId}")]
        public async Task<TResponse> Post(int bookId)
        {
            return await _service.Post(bookId);
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
