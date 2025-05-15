namespace eBooks.Models.Responses
{
    public class ReviewsRes
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string ModifiedAt { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
