namespace eBooks.Models.Responses
{
    public class WishlistRes
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
