namespace eBooks.Models.Responses
{
    public class ReadingProgressRes
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string ModifiedAt { get; set; }
        public int LastReadPage { get; set; }
    }
}
