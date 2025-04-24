using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.User;
using MapsterMapper;

namespace eBooks.Services;

public class UsersService : BaseService<User, UsersSearch, UsersInsertReq, UsersUpdateReq, UsersRes>, IUsersService
{
    public UsersService(EBooksContext db, IMapper mapper) : base(db, mapper)
    {
    }
}
