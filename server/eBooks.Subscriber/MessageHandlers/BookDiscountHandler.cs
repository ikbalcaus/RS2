using eBooks.Models.Messages;
using eBooks.Database;
using Microsoft.EntityFrameworkCore;
using eBooks.Database.Models;
using eBooks.Subscriber.Services;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.MessageHandlers;

namespace eBooks.MessageHandlers
{
    public class BookDiscountHandler : BaseMessageHandler<BookDiscounted>, IMessageHandler<BookDiscounted>
    {
        public BookDiscountHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(BookDiscounted message)
        {
            var emails = await _db.Set<Wishlist>().Include(x => x.User).Where(x => x.BookId == message.Book.BookId).Select(x => x.User.Email).ToListAsync();
            foreach (var email in emails)
            {
                Console.WriteLine($"Sending email to: {email}");
                string subject = $"Book \"{message.Book.Title}\" is on discount";
                string body = $"Book \"{message.Book.Title}\" is on discount";
                await _emailService.SendEmailAsync(email, subject, body);
            }
        }

        public async Task NotifyUser(BookDiscounted message)
        {
            var users = await _db.Set<Wishlist>().Where(x => x.BookId == message.Book.BookId).ToListAsync();
            var userIds = users.Select(x => x.UserId).ToList();
            Console.WriteLine($"Sending notification to users: {string.Join(", ", userIds)}");
            var notifications = userIds.Select(userId => new Notification
            {
                BookId = message.Book.BookId,
                UserId = userId,
                Message = $"Book {message.Book.Title} is on discount"
            }).ToList();
            _db.Set<Notification>().AddRange(notifications);
            await _db.SaveChangesAsync();
        }
    }
}
