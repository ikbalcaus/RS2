namespace eBooks.Models.Responses
{
    public class BookAuthorsRes
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public AuthorsRes Author { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
