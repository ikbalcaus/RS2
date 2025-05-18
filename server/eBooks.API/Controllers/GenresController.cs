using eBooks.API.Auth;
using eBooks.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBooks.Models.Responses;
using eBooks.Models.Requests;
using eBooks.Models.SearchObjects;
using eBooks.Models;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenresController : BaseCRUDController<BaseSearch, GenresReq, GenresReq, GenresRes>
    {
        public GenresController(IGenresService service)
            : base(service)
        {
        }

        [AllowAnonymous]
        public override async Task<PagedResult<GenresRes>> GetPaged([FromQuery] BaseSearch search)
        {
            return await base.GetPaged(search);
        }

        [AllowAnonymous]
        public override async Task<GenresRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "User")]
        public override async Task<GenresRes> Post(GenresReq req)
        {
            return await base.Post(req);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<GenresRes> Put(int id, GenresReq req)
        {
            return await base.Put(id, req);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<GenresRes> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
