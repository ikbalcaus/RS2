namespace eBooks.Models.Books
{
    public class BooksRes
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string PdfPath { get; set; }
        public string StateMachine { get; set; }
        public Publisher Publisher { get; set; }
    }

    public class Publisher
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
