namespace eBooks.Models.Responses
{
    public class AccessRightsRes
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool Hidden { get; set; }
    }
}
