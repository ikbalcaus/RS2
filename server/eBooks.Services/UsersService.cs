using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models;
using MapsterMapper;

namespace eBooks.Services;

public class UsersService : BaseService<UserModel, UserSearchObject, User>, IUsersService
{
    public UsersService(EBooksContext db, IMapper mapper) : base(db, mapper) {}
}
