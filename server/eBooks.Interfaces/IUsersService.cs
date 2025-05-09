using eBooks.Models.User;

namespace eBooks.Interfaces
{
    public interface IUsersService : IBaseService<UsersSearch, UsersCreateReq, UsersUpdateReq, UsersRes>
    {
        Task<UsersRes> UndoDelete(int id);
        Task<UsersRes> ChangeRole(int id, int roleId);
        Task<UsersRes> Login(string email, string password);
    }
}
