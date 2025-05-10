using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController<TSearch, TCreate, TUpdate, TResponse> : BaseReadOnlyController<TSearch, TResponse>
        where TSearch : BaseSearch
        where TResponse : class
    {
        protected new IBaseService<TSearch, TCreate, TUpdate, TResponse> _service;

        public BaseController(IBaseService<TSearch, TCreate, TUpdate, TResponse> service, AccessControlHandler accessControlHandler)
            : base(service, accessControlHandler)
        {
            _service = service;
        }

        [HttpPost]
        public virtual async Task<TResponse> Create(TCreate req)
        {
            return await _service.Create(req);
        }

        [HttpPut("{id}")]
        public virtual async Task<TResponse> Update(int id, TUpdate req)
        {
            return await _service.Update(id, req);
        }

        [HttpDelete("{id}")]
        public virtual async Task<TResponse> Delete(int id)
        {
            return await _service.Delete(id);
        }
    }
}
