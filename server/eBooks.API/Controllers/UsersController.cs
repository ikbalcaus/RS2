using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using eBooks.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseCRUDController<UsersSearch, UsersPostReq, UsersPutReq, UsersRes>
    {
        protected new IUsersService _service;
        protected AccessControlHandler _accessControlHandler;

        public UsersController(IUsersService service, AccessControlHandler accessControlHandler)
            : base(service)
        {
            _service = service;
            _accessControlHandler = accessControlHandler;
        }

        [Authorize(Policy = "Admin")]
        public override async Task<PagedResult<UsersRes>> GetPaged([FromQuery] UsersSearch search)
        {
            return await base.GetPaged(search);
        }

        [Authorize(Policy = "User")]
        public override async Task<UsersRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [AllowAnonymous]
        public override async Task<UsersRes> Post(UsersPostReq req)
        {
            return await base.Post(req);
        }

        [Authorize(Policy = "User")]
        public override async Task<UsersRes> Put(int id, UsersPutReq req)
        {
            await _accessControlHandler.CheckIsOwnerByUserId(id);
            return await base.Put(id, req);
        }

        [Authorize(Policy = "User")]
        public override async Task<UsersRes> Delete(int id)
        {
            await _accessControlHandler.CheckIsOwnerOrAdminByUserId(id);
            return await base.Delete(id);
        }

        [Authorize(Policy = "Admin")]
        [HttpPatch("{id}/undo-delete")]
        public async Task<UsersRes> UndoDelete(int id)
        {
            return await _service.UndoDelete(id);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<LoginRes> Login(string email, string password)
        {
            return await _service.Login(email, password);
        }

        [Authorize(Policy = "User")]
        [HttpPatch("{id}/verify-email/{token}")]
        public async Task<UsersRes> VerifyEmail(int id, string token)
        {
            await _accessControlHandler.CheckIsOwnerByUserId(id);
            return await _service.VerifyEmail(id, token);
        }

        [Authorize(Policy = "Moderator")]
        [HttpPatch("{id}/verify-publisher")]
        public async Task<UsersRes> VerifyPublisher(int id)
        {
            return await _service.VerifyPublisher(id);
        }

        [Authorize(Policy = "Admin")]
        [HttpPatch("{id}/unverify-publisher")]
        public async Task<UsersRes> UnVerifyPublisher(int id)
        {
            return await _service.UnVerifyPublisher(id);
        }
    }
}
