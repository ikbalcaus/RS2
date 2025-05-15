using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseCRUDController<UsersSearch, UsersCreateReq, UsersUpdateReq, UsersRes>
    {
        protected new IUsersService _service;

        public UsersController(IUsersService service, AccessControlHandler accessControlHandler)
            : base(service, accessControlHandler)
        {
        }

        [Authorize(Policy = "Admin")]
        public override async Task<PagedResult<UsersRes>> GetAll([FromQuery] UsersSearch search)
        {
            return await base.GetAll(search);
        }

        [Authorize(Policy = "User")]
        public override async Task<UsersRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [AllowAnonymous]
        public override async Task<UsersRes> Create(UsersCreateReq req)
        {
            return await base.Create(req);
        }

        [Authorize(Policy = "User")]
        public override async Task<UsersRes> Update(int id, UsersUpdateReq req)
        {
            await _accessControlHandler.CheckIsOwnerByUserId(id);
            return await base.Update(id, req);
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

        [Authorize(Policy = "Admin")]
        [HttpPatch("{id}/change-role/{roleId}")]
        public async Task<UsersRes> ChangeRole(int id, int roleId)
        {
            return await _service.ChangeRole(id, roleId);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<UsersRes> Login(string email, string password)
        {
            return await _service.Login(email, password);
        }
    }
}
