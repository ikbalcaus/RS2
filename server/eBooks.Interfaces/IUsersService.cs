using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IUsersService : IBaseCRUDService<UsersSearch, UsersCreateReq, UsersUpdateReq, UsersRes>
    {
        Task<UsersRes> UndoDelete(int id);
        Task<UsersRes> ChangeRole(int id, int roleId);
        Task<UsersRes> Login(string email, string password);
    }
}
