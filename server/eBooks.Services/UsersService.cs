using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.User;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace eBooks.Services;

public class UsersService : BaseService<User, UsersSearch, UsersCreateReq, UsersUpdateReq, UsersRes>, IUsersService
{
    protected ILogger<UsersService> _logger;

    public UsersService(EBooksContext db, IMapper mapper, ILogger<UsersService> logger) : base(db, mapper)
    {
        _logger = logger;
    }

    public override UsersRes Create(UsersCreateReq req)
    {
        _logger.LogInformation($"User with email {req.Email} created.");
        return base.Create(req);
    }

    public override UsersRes Update(int id, UsersUpdateReq req)
    {
        _logger.LogInformation($"User with email {req.Email} updated.");
        return base.Update(id, req);
    }

    public override void Delete(int id)
    {
        var set = _db.Set<User>();
        var entity = set.Find(id);
        _logger.LogInformation($"User with email {entity.Email} deleted.");
        set.Remove(entity);
        _db.SaveChanges();
    }
}
