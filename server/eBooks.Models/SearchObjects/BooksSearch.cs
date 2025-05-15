using eBooks.Models.SearchObjects;

namespace eBooks.Models.Search
{
    public class BooksSearch : BaseSearch
    {
        public string? TitleGTE { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? StateMachine { get; set; }
        public string? AuthorNameGTE { get; set; }
        public string? PublisherNameGTE { get; set; }
        public string? LanguageGTE { get; set; }
        public string? OrderBy { get; set; }
    }
}
