using eBooks.Database.Models;
using eBooks.Models.Responses;

namespace eBooks.Models.Messages
{
    public class PaymentCompleted
    {
        public PurchasesRes Purchase { get; set; }
    }
}
