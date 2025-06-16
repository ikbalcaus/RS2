namespace eBooks.Models.Responses
{
    public class PublisherFollowsRes
    {
        public int UserId { get; set; }
        public UsersRes Publisher { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
