namespace eBooks.Models.Responses
{
    public class PublisherFollowsRes
    {
        public int UserId { get; set; }
        public int PublisherId { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
