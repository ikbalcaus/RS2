namespace eBooks.Models.Responses
{
    public class PurchasesRes
    {
        public int PurchaseId { get; set; }
        public UsersRes User { get; set; }
        public UsersRes Publisher { get; set; }
        public BooksRes Book { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string FailureMessage { get; set; }
        public string FailureCode { get; set; }
        public string FailureReason { get; set; }
    }
}
