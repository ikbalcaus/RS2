namespace eBooks.Models.Responses
{
    public class LoginRes
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FilePath { get; set; }
        public int? PublisherVerifiedById { get; set; }
        public RolesRes Role { get; set; }
    }
}
