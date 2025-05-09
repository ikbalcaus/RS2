using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBooks.Models.Languages;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguagesController : BaseController<BaseSearch, LanguagesCreateReq, LanguagesUpdateReq, LanguagesRes>
    {
        public LanguagesController(ILanguagesService service, AccessControlHandler accessControlHandler)
            : base(service, accessControlHandler)
        {
        }

        [AllowAnonymous]
        public async override Task<PagedResult<LanguagesRes>> GetAll([FromQuery] BaseSearch search)
        {
            return await base.GetAll(search);
        }

        [AllowAnonymous]
        public async override Task<LanguagesRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "User")]
        public async override Task<LanguagesRes> Create(LanguagesCreateReq req)
        {
            return await base.Create(req);
        }

        [Authorize(Policy = "Moderator")]
        public async override Task<LanguagesRes> Update(int id, LanguagesUpdateReq req)
        {
            return await base.Update(id, req);
        }

        [Authorize(Policy = "Moderator")]
        public async override Task<LanguagesRes> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
