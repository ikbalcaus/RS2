using Azure;
using eBooks.Interfaces;
using eBooks.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        protected IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public UsersRes Register(UsersCreateReq req)
        {
            return _service.Register(req);
        }

        [HttpPost("login")]
        public UsersRes Login(string username, string password)
        {
            return _service.Login(username, password);
        }
    }
}
