namespace eBooks.Models.Responses
{
    public class ReadingProgressRes
    {
        public int UserId { get; set; }
        public BooksRes Book { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int LastReadPage { get; set; }
    }
}
