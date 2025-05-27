using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Messages;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.Services;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Subscriber.MessageHandlers
{
    public class PublisherFollowNotificationHandler : BaseMessageHandler<PublisherFollowNotification>, IMessageHandler<PublisherFollowNotification>
    {
        public PublisherFollowNotificationHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(PublisherFollowNotification message)
        {
            var emails = await _db.Set<PublisherFollow>().Where(x => x.PublisherId == message.Book.PublisherId).Include(x => x.User).Include(x => x.Publisher).Select(x => x.User.Email).ToListAsync();
            foreach (var email in emails)
            {
                Console.WriteLine($"Sending email to: {email}");
                string subject = $"Publisher {message.Book.Publisher.UserName} {message.Action} book {message.Book.Title}";
                string body = $"Publisher \"{message.Book.Publisher.UserName}\" {message.Action} book {message.Book.Title}";
                await _emailService.SendEmailAsync(email, subject, body);
            }
        }

        public async Task NotifyUser(PublisherFollowNotification message)
        {
            var userIds = await _db.Set<PublisherFollow>().Where(x => x.PublisherId == message.Book.PublisherId).Select(x => x.UserId).ToListAsync();
            Console.WriteLine($"Sending notification to users: {string.Join(", ", userIds)}");
            var notifications = userIds.Select(userId => new Notification
            {
                PublisherId = message.Book.PublisherId,
                UserId = userId,
                Message = $"Publisher {message.Book.Publisher.UserName} {message.Action} book {message.Book.Title}"
            }).ToList();
            _db.Set<Notification>().AddRange(notifications);
            await _db.SaveChangesAsync();
        }
    }
}
