using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Messages;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.Services;

namespace eBooks.Subscriber.MessageHandlers
{
    public class AccountDeactivatedHandler : BaseMessageHandler<AccountDeactivated>, IMessageHandler<AccountDeactivated>
    {
        public AccountDeactivatedHandler(EBooksContext db, EmailService emailService)
            : base (db, emailService)
        {
        }

        public async Task SendEmail(AccountDeactivated message)
        {
            var email = message.User.Email;
            string notificationMessage;
            if (message.User.DeletionReason != null)
                notificationMessage = $"Your account has been deactivated. Reason: {message.User.DeletionReason}";
            else
                notificationMessage = "Your account has been reactivated";
            await _emailService.SendEmailAsync(email, "Account update", notificationMessage);
        }

        public async Task NotifyUser(AccountDeactivated message)
        {
            string notificationMessage;
            if (message.User.DeletionReason != null)
                notificationMessage = $"Your account has been deactivated. Reason: {message.User.DeletionReason}";
            else
                notificationMessage = "Your account has been reactivated";
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
