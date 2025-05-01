using eBooks.Models.Roles;

namespace eBooks.Models.Users
{
    public class UserRolesRes
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public virtual RolesRes Role { get; set; } = null!;
    }
}
