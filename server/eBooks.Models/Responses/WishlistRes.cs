namespace eBooks.Models.Responses
{
    public class WishlistRes
    {
        public int UserId { get; set; }
        public BooksRes Book { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
