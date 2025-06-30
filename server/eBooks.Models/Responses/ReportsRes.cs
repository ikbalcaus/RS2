namespace eBooks.Models.Responses
{
    public class ReportsRes
    {
        public UsersRes User { get; set; }
        public int BookId { get; set; }
        public BooksRes Book { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string Reason { get; set; }
    }
}
