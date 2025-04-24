using eBooks.Database.Models;
using eBooks.Models.User;

namespace eBooks.Interfaces;

public interface IUsersService : IBaseService<UsersSearch, UsersInsertReq, UsersUpdateReq, UsersRes>
{
}
