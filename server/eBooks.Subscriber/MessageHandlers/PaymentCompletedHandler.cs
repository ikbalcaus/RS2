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
            Console.WriteLine($"Sending email to: {email}");
            await _emailService.SendEmailAsync(email, "Payment completed", "Payment completed");
        }

        public async Task NotifyUser(PaymentCompleted message)
        {
            Console.WriteLine($"Sending notification to user: {message.Purchase.User.UserId}");
            var notification = new Notification
            {
                UserId = message.Purchase.User.UserId,
                BookId = message.Purchase.Book.BookId,
                Message = $"Payment {message.Purchase.PaymentStatus} for book {message.Purchase.Book.Title}"
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
