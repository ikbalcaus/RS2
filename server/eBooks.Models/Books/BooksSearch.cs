namespace eBooks.Models.Books
{
    public class BooksSearch : BaseSearch
    {
        public string? TitleGTE { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
