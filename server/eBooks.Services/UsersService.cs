using System.Security.Cryptography;
using System.Text;
using Azure;
using Azure.Core;
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
        if (!string.IsNullOrWhiteSpace(search?.Email))
        {
            query = query.Where(x => x.Email == search.Email);
        }
        if (!string.IsNullOrWhiteSpace(search?.UNameGTE))
        {
            query = query.Where(x => x.UserName.StartsWith(search.UNameGTE));
        }
        if (search.IsUserRolesIncluded == true)
        {
            query = query.Include(x => x.UserRoles).ThenInclude(x => x.Role);
        }
        return query;
    }

    public override UsersRes Create(UsersCreateReq req)
    {
        _logger.LogInformation($"User with email {req.Email} created.");
        return base.Create(req);
    }

    public override UsersRes Update(int id, UsersUpdateReq req)
    {
        var set = _db.Set<User>();
        var entity = set.Find(id);
        _logger.LogInformation($"User with email {entity.Email} updated.");
        _mapper.Map(req, entity);
        _db.SaveChanges();
        return _mapper.Map<UsersRes>(entity);
    }

    public override void Delete(int id)
    {
        var set = _db.Set<User>();
        var entity = set.Find(id);
        _logger.LogInformation($"User with email {entity.Email} deleted.");
        set.Remove(entity);
        _db.SaveChanges();
    }

    public override void BeforeCreate(User entity, UsersCreateReq req)
    {
        if (!Helpers.IsEmailValid(req.Email)) throw new ExceptionResult("Email is not valid");
        if (_db.Users.Any(x => x.Email == req.Email)) throw new ExceptionResult("Email already exist");
        if (!Helpers.IsPasswordValid(req.Password)) throw new ExceptionResult("Password is not valid");
        entity.PasswordSalt = Helpers.GenerateSalt();
        entity.PasswordHash = Helpers.GenerateHash(entity.PasswordSalt, req.Password);
    }

    public override void BeforeUpdate(User entity, UsersUpdateReq req)
    {
        if (!Helpers.IsPasswordValid(req.Password)) throw new ExceptionResult("Password is not valid");
        if (req.Password != null)
        {
            entity.PasswordSalt = Helpers.GenerateSalt();
            entity.PasswordHash = Helpers.GenerateHash(entity.PasswordSalt, req.Password);
        }
    }
}
