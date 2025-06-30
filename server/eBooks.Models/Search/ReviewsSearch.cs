namespace eBooks.Models.Search
{
    public class ReviewsSearch : BaseSearch
    {
        public int? BookId { get; set; }
        public string? ReviewedBy { get; set; }
        public string? BookTitle { get; set; }
        public bool? IsBookIncluded { get; set; }
        public string? OrderBy { get; set; }
    }
}
