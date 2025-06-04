using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : BaseReadOnlyController<BaseSearch, RolesRes>
    {
        protected new IRolesService _service;

        public RolesController(IRolesService service)
            : base(service)
        {
            _service = service;
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<PagedResult<RolesRes>> GetPaged([FromQuery] BaseSearch search)
        {
            return await base.GetPaged(search);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<RolesRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "Admin")]
        [HttpPatch("{userId}/assign-role/{roleId}")]
        public async Task<UsersRes> AssignRole(int userId, int roleId)
        {
            return await _service.AssignRole(userId, roleId);
        }
    }
}
