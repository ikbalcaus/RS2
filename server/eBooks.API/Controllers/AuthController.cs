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
        public async Task<UsersRes> Register(UsersCreateReq req)
        {
            req.IsRegistering = true;
            return await _service.Create(req);
        }

        [HttpPost("login")]
        public async Task<UsersRes> Login(string email, string password)
        {
            return await _service.Login(email, password);
        }
    }
}
