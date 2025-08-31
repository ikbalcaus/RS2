using eBooks.Models.Messages;
using eBooks.Database;
using eBooks.Subscriber.Services;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.MessageHandlers;
using eBooks.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.MessageHandlers
{
    public class BookReviewedHandler : BaseMessageHandler<BookReviewed>, IMessageHandler<BookReviewed>
    {
        public BookReviewedHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(BookReviewed message)
        {
            var email = message.Book.Publisher.Email;
            string notificationMessage;
            if (message.Book.Status == "Approved")
                notificationMessage = "Your book has been approved";
            else
                notificationMessage = $"Your book has been rejected. Reason: {message.Book.RejectionReason}";
            Console.WriteLine($"Sending email to: {email}");
            await _emailService.SendEmailAsync(email, "Book reviewed", notificationMessage);
        }

        public async Task NotifyUser(BookReviewed message)
        {
            string notificationMessage;
            if (message.Book.Status == "Approved")
                notificationMessage = "Your book has been approved";
            else
                notificationMessage = $"Your book has been rejected. Reason: {message.Book.RejectionReason}";
            var user = await _db.Set<User>().FirstOrDefaultAsync(x => x.UserId == message.Book.Publisher.UserId);
            Console.WriteLine($"Sending notification to user: {user.UserId}");
            var notification = new Notification
            {
                UserId = user.UserId,
                BookId = message.Book.BookId,
                Message = notificationMessage
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
