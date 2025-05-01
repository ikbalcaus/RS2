using eBooks.Models.Users;

namespace eBooks.Models.User
{
    public class UsersRes
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public virtual ICollection<UserRolesRes> UserRoles { get; set; } = new List<UserRolesRes>();
    }
}
