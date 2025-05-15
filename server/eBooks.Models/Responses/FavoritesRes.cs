namespace eBooks.Models.Responses
{
    public class FavoritesRes
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string ModifiedAt { get; set; }
    }
}
