using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBooks.Models.Responses;
using eBooks.Models.Requests;
using eBooks.Models.SearchObjects;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguagesController : BaseCRUDController<BaseSearch, LanguagesPostReq, LanguagesPutReq, LanguagesRes>
    {
        public LanguagesController(ILanguagesService service)
            : base(service)
        {
        }

        [AllowAnonymous]
        public override async Task<PagedResult<LanguagesRes>> GetPaged([FromQuery] BaseSearch search)
        {
            return await base.GetPaged(search);
        }

        [AllowAnonymous]
        public override async Task<LanguagesRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "User")]
        public override async Task<LanguagesRes> Post(LanguagesPostReq req)
        {
            return await base.Post(req);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<LanguagesRes> Put(int id, LanguagesPutReq req)
        {
            return await base.Put(id, req);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<LanguagesRes> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
