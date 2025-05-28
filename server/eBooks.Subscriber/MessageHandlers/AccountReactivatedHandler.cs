using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Messages;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.Services;

namespace eBooks.Subscriber.MessageHandlers
{
    public class AccountReactivatedHandler : BaseMessageHandler<AccountReactivated>, IMessageHandler<AccountReactivated>
    {
        public AccountReactivatedHandler(EBooksContext db, EmailService emailService)
            : base (db, emailService)
        {
        }

        public async Task SendEmail(AccountReactivated message)
        {
            var email = message.User.Email;
            var notificationMessage = $"Your account is reactivated. Reason for deactivating: {message.User.DeleteReason}";
            Console.WriteLine($"Sending email to: {email}");
            await _emailService.SendEmailAsync(email, "Account reactivated", notificationMessage);
        }

        public async Task NotifyUser(AccountReactivated message)
        {
            var notificationMessage = $"Your account is reactivated. Reason for deactivating: {message.User.DeleteReason}";
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
