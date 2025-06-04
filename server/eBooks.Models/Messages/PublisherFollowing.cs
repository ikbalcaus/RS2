using eBooks.Models.Responses;

namespace eBooks.Models.Messages
{
    public class PublisherFollowing
    {
        public BooksRes Book { get; set; }
        public string Action { get; set; }
    }
}
