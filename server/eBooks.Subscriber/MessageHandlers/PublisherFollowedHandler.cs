using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Messages;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.Services;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Subscriber.MessageHandlers
{
    public class PublisherFollowingHandler : BaseMessageHandler<PublisherFollowing>, IMessageHandler<PublisherFollowing>
    {
        public PublisherFollowingHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(PublisherFollowing message)
        {
            var emails = await _db.Set<PublisherFollow>().Where(x => x.PublisherId == message.Book.Publisher.UserId).Include(x => x.User).Include(x => x.Publisher).Select(x => x.User.Email).ToListAsync();
            foreach (var email in emails)
            {
                Console.WriteLine($"Sending email to: {email}");
                string subject = $"Publisher {message.Book.Publisher.UserName} {message.Action} book {message.Book.Title}";
                string notificationMessage = $"Publisher \"{message.Book.Publisher.UserName}\" {message.Action} book {message.Book.Title}";
                await _emailService.SendEmailAsync(email, subject, notificationMessage);
            }
        }

        public async Task NotifyUser(PublisherFollowing message)
        {
            var userIds = await _db.Set<PublisherFollow>().Where(x => x.PublisherId == message.Book.Publisher.UserId).Select(x => x.UserId).ToListAsync();
            Console.WriteLine($"Sending notification to users: {string.Join(", ", userIds)}");
            var notifications = userIds.Select(userId => new Notification
            {
                PublisherId = message.Book.Publisher.UserId,
                UserId = userId,
                Message = $"Publisher {message.Book.Publisher.UserName} {message.Action} book {message.Book.Title}"
            }).ToList();
            _db.Set<Notification>().AddRange(notifications);
            await _db.SaveChangesAsync();
        }
    }
}
