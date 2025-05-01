using eBooks.Models.User;

namespace eBooks.Interfaces
{
    public interface IAuthService
    {
        public UsersRes Register(UsersCreateReq req);
        public UsersRes Login(string email, string password);
    }
}
