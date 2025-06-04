namespace eBooks.Models.Responses
{
    public class UsersRes
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int? PublisherVerifiedById { get; set; }
        public string DeletionReason { get; set; }
    }
}
