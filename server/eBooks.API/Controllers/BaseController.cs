using eBooks.Models;
using eBooks.Services;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController<TModel, TSearch> : ControllerBase where TSearch : BaseSearchObject
    {
        protected IBaseService<TModel, TSearch> _service;

        public BaseController(IBaseService<TModel, TSearch> service)
        {
            _service = service;
        }

        [HttpGet]
        public PagedResult<TModel> GetAll([FromQuery] TSearch searchObject)
        {
            return _service.GetPaged(searchObject);
        }

        [HttpGet("{id}")]
        public TModel GetById(int id)
        {
            return _service.GetById(id);
        }
    }
}
