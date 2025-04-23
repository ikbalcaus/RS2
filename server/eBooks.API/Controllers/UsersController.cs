using eBooks.Interfaces;
using eBooks.Models;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController<UserModel, UserSearchObject>
    {
        public UsersController(IUsersService service) : base(service) {}
    }
}
