using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IUsersService : IBaseCRUDService<UsersSearch, UsersPostReq, UsersPutReq, UsersRes>
    {
        Task<UsersRes> DeleteByAdmin(int id, string? reason);
        Task<LoginRes> Login(LoginReq req);
        Task<UsersRes> ForgotPassword(string email);
        Task<UsersRes> ResetPassword(string token, string password);
        Task<UsersRes> VerifyEmail(int id, string? token);
        Task<UsersRes> VerifyPublisher(int id);
    }
}
