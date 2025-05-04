using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.User;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBooks.Services;

public class UsersService : BaseService<User, UsersSearch, UsersCreateReq, UsersUpdateReq, UsersRes>, IUsersService
{
    protected ILogger<UsersService> _logger;

    public UsersService(EBooksContext db, IMapper mapper, ILogger<UsersService> logger) : base(db, mapper)
    {
        _logger = logger;
    }

    public override IQueryable<User> AddFilter(UsersSearch search, IQueryable<User> query)
    {
        if (!string.IsNullOrWhiteSpace(search?.FNameGTE))
        {
            query = query.Where(x => x.FirstName.StartsWith(search.FNameGTE));
        }
        if (!string.IsNullOrWhiteSpace(search?.LNameGTE))
        {
            query = query.Where(x => x.LastName.StartsWith(search.LNameGTE));
        }
        if (!string.IsNullOrWhiteSpace(search?.EmailGTE))
        {
            query = query.Where(x => x.Email.StartsWith(search.EmailGTE));
        }
        if (!string.IsNullOrWhiteSpace(search?.UNameGTE))
        {
            query = query.Where(x => x.UserName.StartsWith(search.UNameGTE));
        }
        return query;
    }

    public override UsersRes Create(UsersCreateReq req)
    {
        return base.Create(req);
    }

    public override UsersRes Update(int id, UsersUpdateReq req)
    {
        return base.Update(id, req);
    }

    public override UsersRes Delete(int id)
    {
        return base.Delete(id);
    }

    public UsersRes Login(string email, string password)
    {
        var entity = _db.Set<User>().Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefault(x => x.Email == email);
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

    public override void BeforeCreate(User entity, UsersCreateReq req)
    {
        if (!Helpers.IsEmailValid(req.Email)) throw new ExceptionResult("Email is not valid");
        if (_db.Users.Any(x => x.Email == req.Email)) throw new ExceptionResult("Email already exist");
        if (!Helpers.IsPasswordValid(req.Password)) throw new ExceptionResult("Password is not valid");
        entity.PasswordSalt = Helpers.GenerateSalt();
        entity.PasswordHash = Helpers.GenerateHash(entity.PasswordSalt, req.Password);
        Role role;
        if (req.IsRegistering == true) role = _db.Set<Role>().FirstOrDefault(x => x.Name == "User");
        else role = _db.Set<Role>().FirstOrDefault(x => x.RoleId == req.RoleId);
        var userRole = new UserRole
        {
            RoleId = role.RoleId,
            User = entity,
            AssignedAt = DateTime.Now
        };
        entity.UserRoles.Add(userRole);
        _logger.LogInformation($"User with email {req.Email} created.");
    }

    public override void BeforeUpdate(User entity, UsersUpdateReq req)
    {
        if (req.Password != null)
        {
            if (!Helpers.IsPasswordValid(req.Password)) throw new ExceptionResult("Password is not valid");
            entity.PasswordSalt = Helpers.GenerateSalt();
            entity.PasswordHash = Helpers.GenerateHash(entity.PasswordSalt, req.Password);
        }
        _logger.LogInformation($"User with email {entity.Email} updated.");
    }

    public override void BeforeDelete(User entity, int id)
    {
        _logger.LogInformation($"User with email {entity.Email} deleted.");
    }
}
