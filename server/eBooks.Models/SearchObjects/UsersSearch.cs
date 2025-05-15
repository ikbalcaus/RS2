using eBooks.Models.SearchObjects;

namespace eBooks.Models.Search
{
    public class UsersSearch : BaseSearch
    {
        public string? FNameGTE { get; set; }
        public string? LNameGTE { get; set; }
        public string? EmailGTE { get; set; }
        public string? UNameGTE { get; set; }
        public string? OrderBy { get; set; }
    }
}
