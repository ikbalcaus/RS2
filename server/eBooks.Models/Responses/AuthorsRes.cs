namespace eBooks.Models.Responses
{
    public class AuthorsRes
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public UsersRes? ModifiedBy { get; set; }
    }
}
