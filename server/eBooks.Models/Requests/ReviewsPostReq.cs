namespace eBooks.Models.Requests
{
    public class ReviewsPostReq
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
