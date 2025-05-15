namespace eBooks.Models.Responses
{
    public class BooksRes
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string PdfPath { get; set; }
        public int NumberOfPages { get; set; }
        public string StateMachine { get; set; }
        public UsersRes Publisher { get; set; }
        public LanguagesRes Language { get; set; }
        public ICollection<BookImageRes> BookImages { get; set; } = new List<BookImageRes>();
    }
}
