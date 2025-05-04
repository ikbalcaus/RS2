using eBooks.Interfaces;
using eBooks.Models;
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
        public UsersController(IUsersService service) : base(service)
        {
        }

        [Authorize(Roles = "Admin")]
        public override PagedResult<UsersRes> GetAll([FromQuery] UsersSearch search)
        {
            return base.GetAll(search);
        }

        [Authorize(Roles = "Admin")]
        public override UsersRes GetById(int id)
        {
            return base.GetById(id);
        }


        [Authorize(Roles = "Admin")]
        public override UsersRes Create(UsersCreateReq req)
        {
            return base.Create(req);
        }

        [Authorize(Roles = "User,Publisher,Admin,Moderator")]
        public override UsersRes Update(int id, UsersUpdateReq req)
        {
            return base.Update(id, req);
        }

        [Authorize(Roles = "User,Publisher,Admin,Moderator")]
        public override UsersRes Delete(int id)
        {
            return base.Delete(id);
        }
    }
}
