using eBooks.Models.User;

namespace eBooks.Models.Books
{
    public class BooksRes
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string PdfPath { get; set; }
        public string StateMachine { get; set; }
        public UsersRes Publisher { get; set; }
    }
}
