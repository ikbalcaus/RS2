using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccessRightsController : BaseUserContextController<BaseSearch, object, AccessRightsRes>
    {
        protected new IAccessRightsService _service;

        public AccessRightsController(IAccessRightsService service)
            : base(service)
        {
            _service = service;
        }

        [Authorize(Policy = "User")]
        [HttpPatch("{bookId}/favorite")]
        public async Task<AccessRightsRes> ToggleFavorite(int bookId)
        {
            return await _service.ToggleFavorite(bookId);
        }
    }
}
