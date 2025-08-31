namespace eBooks.Models.Responses
{
    public class ReviewsRes
    {
        public UsersRes User { get; set; }
        public int BookId { get; set; }
        public BooksRes Book { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public UsersRes? ReportedBy { get; set; }
        public string? ReportReason { get; set; }
    }
}
