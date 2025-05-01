using eBooks.Interfaces;
using eBooks.Models;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController<TSearch, TCreate, TUpdate, TResponse> : BaseReadOnlyController<TSearch, TResponse> where TSearch : BaseSearch where TResponse : class
    {
        protected new IBaseService<TSearch, TCreate, TUpdate, TResponse> _service;

        public BaseController(IBaseService<TSearch, TCreate, TUpdate, TResponse> service) : base(service)
        {
            _service = service;
        }

        [HttpPost]
        public virtual TResponse Create(TCreate req)
        {
            return _service.Create(req);
        }

        [HttpPut("{id}")]
        public virtual TResponse Update(int id, TUpdate req)
        {
            return _service.Update(id, req);
        }

        [HttpDelete("{id}")]
        public virtual TResponse Delete(int id)
        {
            return _service.Delete(id);
        }
    }
}
