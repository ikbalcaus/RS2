namespace eBooks.Models.Responses
{
    public class BookGenresRes
    {
        public int BookId { get; set; }
        public int GenreId { get; set; }
        public GenresRes Genre { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
