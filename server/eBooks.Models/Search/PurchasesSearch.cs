namespace eBooks.Models.Search
{
    public class PurchasesSearch : BaseSearch
    {
        public string? User { get; set; }
        public string? Publisher { get; set; }
        public string? Book { get; set; }
        public string? OrderBy { get; set; }
    }
}
