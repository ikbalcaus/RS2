namespace eBooks.Models.Requests
{
    public class DiscountReq
    {
        public int DiscountPercentage { get; set; }

        public DateTime DiscountStart { get; set; }

        public DateTime DiscountEnd { get; set; }
    }
}
