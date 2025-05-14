using eBooks.Interfaces;
using eBooks.Models.AccessRights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccessRightsController : ControllerBase
    {
        protected IAccessRightsService _service;

        public AccessRightsController(IAccessRightsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        public async Task<List<AccessRightsRes>> GetAll()
        {
            return await _service.GetAll();
        }

        [HttpGet("{bookId}")]
        [Authorize(Policy = "User")]
        public async Task<AccessRightsRes> GetById(int bookId)
        {
            return await _service.GetById(bookId);
        }

        [HttpPost("{bookId}")]
        [Authorize(Policy = "User")]
        public async Task<AccessRightsRes> Post(int bookId)
        {
            return await _service.Post(bookId);
        }

        [HttpPatch("{bookId}")]
        [Authorize(Policy = "User")]
        public async Task<AccessRightsRes> Patch(int bookId)
        {
            return await _service.Patch(bookId);
        }

        [HttpDelete("{bookId}")]
        [Authorize(Policy = "User")]
        public async Task<AccessRightsRes> Delete(int bookId)
        {
            return await _service.Delete(bookId);
        }
    }
}
