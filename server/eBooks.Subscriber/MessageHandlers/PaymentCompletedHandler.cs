using eBooks.Models.Messages;
using eBooks.Database;
using eBooks.Subscriber.Services;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.MessageHandlers;
using eBooks.Database.Models;

namespace eBooks.MessageHandlers
{
    public class PaymentCompletedHandler : BaseMessageHandler<PaymentCompleted>, IMessageHandler<PaymentCompleted>
    {
        public PaymentCompletedHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(PaymentCompleted message)
        {
            var email = message.Purchase.User.Email;
            var notificationMessage = $"Payment {message.Purchase.PaymentStatus}.<br>Purchased book: {message.Purchase.Book.Title}<br>Total price: {message.Purchase.TotalPrice}";
            Console.WriteLine($"Sending email to: {email}");
            await _emailService.SendEmailAsync(email, $"Payment {message.Purchase.PaymentStatus}", notificationMessage);
        }

        public async Task NotifyUser(PaymentCompleted message)
        {
            Console.WriteLine($"Sending notification to user: {message.Purchase.User.UserId}");
            var notification = new Notification
            {
                UserId = message.Purchase.User.UserId,
                BookId = message.Purchase.Book.BookId,
                Message = $"Payment {message.Purchase.PaymentStatus}. Purchased book: {message.Purchase.Book.Title}. Total price: {message.Purchase.TotalPrice}"
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
