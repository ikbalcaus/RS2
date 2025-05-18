using eBooks.API.Auth;
using eBooks.Models;
using eBooks.Models.SearchObjects;
using eBooks.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BaseReadOnlyController<TSearch, TResponse> : ControllerBase
        where TSearch : BaseSearch
    {
        protected IBaseReadOnlyService<TSearch, TResponse> _service;

        public BaseReadOnlyController(IBaseReadOnlyService<TSearch, TResponse> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<PagedResult<TResponse>> GetPaged([FromQuery] TSearch search)
        {
            return await _service.GetPaged(search);
        }

        [HttpGet("{id}")]
        public virtual async Task<TResponse> GetById(int id)
        {
            return await _service.GetById(id);
        }
    }
}
