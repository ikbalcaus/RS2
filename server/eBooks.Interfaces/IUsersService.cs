using eBooks.Models;

namespace eBooks.Interfaces;

public interface IUsersService
{
    List<UsersModel> Get();
}
