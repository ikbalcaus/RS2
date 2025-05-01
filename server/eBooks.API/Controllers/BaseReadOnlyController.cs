using eBooks.Models;
using eBooks.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize(Roles = "")]
    [ApiController]
    [Route("[controller]")]
    public class BaseReadOnlyController<TSearch, TResponse> : ControllerBase where TSearch : BaseSearch
    {
        protected IBaseReadOnlyService<TSearch, TResponse> _service;

        public BaseReadOnlyController(IBaseReadOnlyService<TSearch, TResponse> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual PagedResult<TResponse> GetAll([FromQuery] TSearch search)
        {
            return _service.GetPaged(search);
        }

        [HttpGet("{id}")]
        public virtual TResponse GetById(int id)
        {
            return _service.GetById(id);
        }
    }
}
