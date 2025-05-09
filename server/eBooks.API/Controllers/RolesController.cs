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
        public override Task<PagedResult<RolesRes>> GetAll([FromQuery] BaseSearch search)
        {
            return base.GetAll(search);
        }

        [Authorize(Policy = "Moderator")]
        public override Task<RolesRes> GetById(int id)
        {
            return base.GetById(id);
        }
    }
}
