namespace eBooks.Models.Search
{
    public class UsersSearch : BaseSearch
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? IsDeleted { get; set; }
        public string? OrderBy { get; set; }
    }
}
