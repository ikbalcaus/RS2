using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : BaseReadOnlyController<BaseSearch, RolesRes>
    {
        public RolesController(IRolesService service, AccessControlHandler accessControlHandler)
            : base(service, accessControlHandler)
        {
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<PagedResult<RolesRes>> GetAll([FromQuery] BaseSearch search)
        {
            return await base.GetAll(search);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<RolesRes> GetById(int id)
        {
            return await base.GetById(id);
        }
    }
}
