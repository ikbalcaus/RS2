namespace eBooks.Models.Search
{
    public class ReviewsSearch : BaseSearch
    {
        public int? BookId { get; set; }
        public string? ReviewedBy { get; set; }
        public string? BookTitle { get; set; }
        public string? IsReported { get; set; }
        public bool? IsBookIncluded { get; set; }
        public bool? IsReportedByIncluded { get; set; }
        public string? OrderBy { get; set; }
    }
}
