using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.User;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBooks.Services
{
    public class AuthService : IAuthService
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected ILogger<AuthService> _logger;

        public AuthService(EBooksContext db, IMapper mapper, ILogger<AuthService> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        public UsersRes Register(UsersCreateReq req)
        {
            _logger.LogInformation($"User with email {req.Email} registered.");
            throw new NotImplementedException();
        }

        public UsersRes Login(string email, string password)
        {
            var entity = _db.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefault(x => x.Email == email);
            if (entity == null)
            {
                _logger.LogInformation($"User failed to log in {email}.");
                return null;
            }
            if (Helpers.GenerateHash(entity.PasswordSalt, password) != entity.PasswordHash)
            {
                _logger.LogInformation($"User with email {email} failed to log in.");
                return null;
            }
            _logger.LogInformation($"User with email {email} logged in.");
            return _mapper.Map<UsersRes>(entity);
        }
    }
}
