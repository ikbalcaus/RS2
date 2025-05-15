namespace eBooks.Models.Responses
{
    public class AccessRightsRes
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string ModifiedAt { get; set; }
        public bool Hidden { get; set; }
        public BooksRes Book { get; set; }
    }
}
