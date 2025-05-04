using eBooks.Interfaces;
using eBooks.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        protected IUsersService _service;

        public AuthController(IUsersService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public UsersRes Register(UsersCreateReq req)
        {
            req.IsRegistering = true;
            return _service.Create(req);
        }

        [HttpPost("login")]
        public UsersRes Login(string email, string password)
        {
            return _service.Login(email, password);
        }
    }
}
