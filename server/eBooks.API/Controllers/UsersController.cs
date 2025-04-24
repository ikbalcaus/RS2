using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController<UsersSearch, UsersInsertReq, UsersUpdateReq, UsersRes>
    {
        public UsersController(IUsersService service) : base(service)
        {
        }
    }
}
