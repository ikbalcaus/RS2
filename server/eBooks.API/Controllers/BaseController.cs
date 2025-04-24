using eBooks.Interfaces;
using eBooks.Models;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    public class BaseController<TSearch, TInsert, TUpdate, TResponse> : BaseReadOnlyController<TSearch, TResponse> where TSearch : BaseSearch where TResponse : class
    {
        protected new IBaseService<TSearch, TInsert, TUpdate, TResponse> _service;

        public BaseController(IBaseService<TSearch, TInsert, TUpdate, TResponse> service) : base(service)
        {
            _service = service;
        }

        [HttpPost]
        public TResponse Insert(TInsert req)
        {
            return _service.Insert(req);
        }

        [HttpPut("{id}")]
        public TResponse Update(int id, TUpdate req)
        {
            return _service.Update(id, req);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _service.Delete(id);
        }
    }
}
