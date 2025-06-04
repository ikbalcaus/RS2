namespace eBooks.Models.Responses
{
    public class BooksRes
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public bool HasAccessRight { get; set; }
        public decimal Price { get; set; }
        public int NumberOfPages { get; set; }
        public int NumberOfViews { get; set; }
        public string StateMachine { get; set; }
        public string RejectionReason { get; set; }
        public string DeletionReason { get; set; }
        public DateTime ModifiedAt { get; set; }
        public UsersRes Publisher { get; set; }
        public LanguagesRes Language { get; set; }
        public int? DiscountPercentage { get; set; }
        public DateTime? DiscountStart { get; set; }
        public DateTime? DiscountEnd { get; set; }
    }
}
