using eBooks.Models.Responses;

namespace eBooks.Models.Messages
{
    public class PublisherFollowNotification
    {
        public BooksRes Book { get; set; }
        public string Action { get; set; }
    }
}
