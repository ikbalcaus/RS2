namespace eBooks.Models.Responses
{
    public class ReviewsRes
    {
        public int UserId { get; set; }
        public BooksRes Book { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
