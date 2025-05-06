using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize(Roles = "Admin,Moderator,User,Publisher")]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController<UsersSearch, UsersCreateReq, UsersUpdateReq, UsersRes>
    {
        public UsersController(IUsersService service, IAuthorizationService authService)
            : base(service, authService)
        {
        }

        [Authorize(Policy = "Admin")]
        public async override Task<PagedResult<UsersRes>> GetAll([FromQuery] UsersSearch search)
        {
            return await base.GetAll(search);
        }

        [Authorize(Policy = "User")]
        public async override Task<UsersRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "Admin")]
        public async override Task<UsersRes> Create(UsersCreateReq req)
        {
            return await base.Create(req);
        }

        [Authorize(Policy = "User")]
        public async override Task<UsersRes> Update(int id, UsersUpdateReq req)
        {
            if (!(await _authService.AuthorizeAsync(User, id, "OwnerOrAdmin")).Succeeded)
                throw new ExceptionForbidden("Only owner or admin can use this action");
            return await base.Update(id, req);
        }

        [Authorize(Policy = "User")]
        public async override Task<UsersRes> Delete(int id)
        {
            if (!(await _authService.AuthorizeAsync(User, id, "OwnerOrAdmin")).Succeeded)
                throw new ExceptionForbidden("Only owner or admin can use this action");
            return await base.Delete(id);
        }
    }
}
