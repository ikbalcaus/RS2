namespace eBooks.Models.Responses
{
    public class BookAuthorsRes
    {
        public int BookId { get; set; }
        public AuthorsRes Author { get; set; }
    }
}
