using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IUsersService : IBaseCRUDService<UsersSearch, UsersPostReq, UsersPutReq, UsersRes>
    {
        Task<UsersRes> UndoDelete(int id);
        Task<UsersRes> Login(string email, string password);
    }
}
