using eBooks.Models.User;

namespace eBooks.Interfaces
{
    public interface IUsersService : IBaseService<UsersSearch, UsersCreateReq, UsersUpdateReq, UsersRes>
    {
        public UsersRes Login(string email, string password);
    }
}
