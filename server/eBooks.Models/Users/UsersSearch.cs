namespace eBooks.Models.User
{
    public class UsersSearch : BaseSearch
    {
        public string? FNameGTE { get; set; }
        public string? LNameGTE { get; set; }
        public string? EmailGTE { get; set; }
        public string? UNameGTE { get; set; }
    }
}
