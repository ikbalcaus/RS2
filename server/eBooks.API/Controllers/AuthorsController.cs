using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBooks.Models.Authors;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : BaseController<BaseSearch, AuthorsCreateReq, AuthorsUpdateReq, AuthorsRes>
    {
        public AuthorsController(IAuthorsService service, AccessControlHandler accessControlHandler)
            : base(service, accessControlHandler)
        {
        }

        [AllowAnonymous]
        public override async Task<PagedResult<AuthorsRes>> GetAll([FromQuery] BaseSearch search)
        {
            return await base.GetAll(search);
        }

        [AllowAnonymous]
        public override async Task<AuthorsRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "User")]
        public override async Task<AuthorsRes> Create(AuthorsCreateReq req)
        {
            return await base.Create(req);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<AuthorsRes> Update(int id, AuthorsUpdateReq req)
        {
            return await base.Update(id, req);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<AuthorsRes> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
