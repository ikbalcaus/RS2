namespace eBooks.Models.Responses
{
    public class GenresRes
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public UsersRes? ModifiedBy { get; set; }
    }
}
