namespace eBooks.Models.Search
{
    public class BooksSearch : BaseSearch
    {
        public string? Title { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public string? Language { get; set; }
        public int? PublisherId { get; set; }
        public string? Publisher { get; set; }
        public string? Status { get; set; }
        public string? IsDeleted { get; set; }
        public bool? FollowingPublishersOnly { get; set; }
        public string? OrderBy { get; set; }
        public bool? IsAuthorsIncluded { get; set; }
        public bool? IsGenresIncluded { get; set; }
    }
}
