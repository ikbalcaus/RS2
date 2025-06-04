namespace eBooks.Models.Search
{
    public class AuthorsSearch : BaseSearch
    {
        public string? Name { get; set; }
        public string? OrderBy { get; set; }
    }
}
