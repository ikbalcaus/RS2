namespace eBooks.Models.Requests
{
    public class ReviewsPutReq
    {
        public int? Rating { get; set; }
        public string? Comment { get; set; }
    }
}
