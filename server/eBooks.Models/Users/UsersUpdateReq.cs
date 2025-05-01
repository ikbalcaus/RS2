namespace eBooks.Models.User
{
    public class UsersUpdateReq
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int RoleId { get; set; }
    }
}
