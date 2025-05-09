using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models.Roles;
using eBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBooks.Models.Genres;
using eBooks.Models.User;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenresController : BaseController<BaseSearch, GenresReq, GenresReq, GenresRes>
    {
        public GenresController(IGenresService service, AccessControlHandler accessControlHandler)
            : base(service, accessControlHandler)
        {
        }

        [AllowAnonymous]
        public async override Task<PagedResult<GenresRes>> GetAll([FromQuery] BaseSearch search)
        {
            return await base.GetAll(search);
        }

        [AllowAnonymous]
        public async override Task<GenresRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "User")]
        public async override Task<GenresRes> Create(GenresReq req)
        {
            return await base.Create(req);
        }

        [Authorize(Policy = "Moderator")]
        public async override Task<GenresRes> Update(int id, GenresReq req)
        {
            return await base.Update(id, req);
        }

        [Authorize(Policy = "Moderator")]
        public async override Task<GenresRes> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
