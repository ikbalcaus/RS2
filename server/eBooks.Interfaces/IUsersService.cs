using eBooks.Models;
using eBooks.Services;

namespace eBooks.Interfaces;

public interface IUsersService : IBaseService<UserModel, UserSearchObject>
{
}
