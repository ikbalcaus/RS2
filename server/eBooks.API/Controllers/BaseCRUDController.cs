using eBooks.Interfaces;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BaseCRUDController<TSearch, TCreate, TUpdate, TResponse> : BaseReadOnlyController<TSearch, TResponse>
        where TSearch : BaseSearch
        where TResponse : class
    {
        protected new IBaseCRUDService<TSearch, TCreate, TUpdate, TResponse> _service;

        public BaseCRUDController(IBaseCRUDService<TSearch, TCreate, TUpdate, TResponse> service)
            : base(service)
        {
            _service = service;
        }

        [HttpPost]
        public virtual async Task<TResponse> Post(TCreate req)
        {
            return await _service.Post(req);
        }

        [HttpPut("{id}")]
        public virtual async Task<TResponse> Put(int id, TUpdate req)
        {
            return await _service.Put(id, req);
        }

        [HttpDelete("{id}")]
        public virtual async Task<TResponse> Delete(int id)
        {
            return await _service.Delete(id);
        }
    }
}
