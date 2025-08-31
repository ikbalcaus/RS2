using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Messages;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.Services;

namespace eBooks.Subscriber.MessageHandlers
{
    public class PublisherVerifiedHandler : BaseMessageHandler<PublisherVerified>, IMessageHandler<PublisherVerified>
    {
        public PublisherVerifiedHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(PublisherVerified message)
        {
            var email = message.User.Email;
            string notificationMessage;
            if (message.User.PublisherVerifiedById != null)
                notificationMessage = $"Your account has been verified";
            else
                notificationMessage = "Your account has been unverified";
            Console.WriteLine($"Sending email to: {email}");
            await _emailService.SendEmailAsync(email, "Account update", notificationMessage);
        }

        public async Task NotifyUser(PublisherVerified message)
        {
            string notificationMessage;
            if (message.User.PublisherVerifiedById != null)
                notificationMessage = $"Your account has been verified";
            else
                notificationMessage = "Your account has been unverified";
            Console.WriteLine($"Sending notification to user: {message.User.UserId}");
            var notification = new Notification
            {
                UserId = message.User.UserId,
                PublisherId = message.User.UserId,
                Message = notificationMessage
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
