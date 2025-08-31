namespace eBooks.Models.Responses
{
    public class AccessRightsRes
    {
        public int UserId { get; set; }
        public BooksRes Book { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsHidden { get; set; }
        public int LastReadPage { get; set; }
    }
}
