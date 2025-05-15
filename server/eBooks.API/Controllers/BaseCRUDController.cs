using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models.SearchObjects;
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

        public BaseCRUDController(IBaseCRUDService<TSearch, TCreate, TUpdate, TResponse> service, AccessControlHandler accessControlHandler)
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
