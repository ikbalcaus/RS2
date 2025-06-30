namespace eBooks.Models.Search
{
    public class ReportsSearch : BaseSearch
    {
        public string? ReportedBy { get; set; }
        public string? BookTitle { get; set; }
        public bool? IsBookIncluded { get; set; }
        public string? OrderBy { get; set; }
    }
}
