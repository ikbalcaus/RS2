using eBooks.API.Auth;
using eBooks.Models;
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
        protected AccessControlHandler _accessControlHandler;

        public BaseReadOnlyController(IBaseReadOnlyService<TSearch, TResponse> service, AccessControlHandler accessControlHandler)
        {
            _service = service;
            _accessControlHandler = accessControlHandler;
        }

        [HttpGet]
        public async virtual Task<PagedResult<TResponse>> GetAll([FromQuery] TSearch search)
        {
            return await _service.GetPaged(search);
        }

        [HttpGet("{id}")]
        public async virtual Task<TResponse> GetById(int id)
        {
            return await _service.GetById(id);
        }
    }
}
