using eBooks.Interfaces;
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
    }
}
