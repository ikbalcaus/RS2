using eBooks.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBooks.Models.Search;
using eBooks.Models.Requests;
using eBooks.Models.Responses;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : BaseCRUDController<AuthorsSearch, AuthorsReq, AuthorsReq, AuthorsRes>
    {
        public AuthorsController(IAuthorsService service)
            : base(service)
        {
        }

        [AllowAnonymous]
        public override async Task<PagedResult<AuthorsRes>> GetPaged([FromQuery] AuthorsSearch search)
        {
            return await base.GetPaged(search);
        }

        [AllowAnonymous]
        public override async Task<AuthorsRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<AuthorsRes> Post(AuthorsReq req)
        {
            return await base.Post(req);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<AuthorsRes> Put(int id, AuthorsReq req)
        {
            return await base.Put(id, req);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<AuthorsRes> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
