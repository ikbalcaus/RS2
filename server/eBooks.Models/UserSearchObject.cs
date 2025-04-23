namespace eBooks.Models
{
    public class UserSearchObject : BaseSearchObject
    {
        public string? FNameGTE { get; set; }
        public string? LNameGTE { get; set; }
        public string? Email { get; set; }
        public string? UNameGTE { get; set; }
        public bool? IsUserRolesIncluded { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? OrderBy { get; set; }
    }
}
