using eBooks.Models.Responses;

namespace eBooks.Models.Messages
{
    public class BookReviewed
    {
        public BooksRes Book { get; set; }
        public string Status { get; set; }
    }
}
