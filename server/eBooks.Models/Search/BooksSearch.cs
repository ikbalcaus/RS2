namespace eBooks.Models.Search
{
    public class BooksSearch : BaseSearch
    {
        public string? Title { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? StateMachine { get; set; }
        public string? Author { get; set; }
        public string? PublisherName { get; set; }
        public string? Language { get; set; }
        public string? OrderBy { get; set; }
    }
}
