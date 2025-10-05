using eBooks.Models.Messages;
using eBooks.Database;
using eBooks.Subscriber.Services;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.MessageHandlers;
using eBooks.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.MessageHandlers
{
    public class EmailVerificationHandler : BaseMessageHandler<EmailVerification>, IMessageHandler<EmailVerification>
    {
        public EmailVerificationHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(EmailVerification message)
        {
            var email = message.Token.Email;
            var notificationMessage = $"Enter following token in order to verify your email: {message.Token.VerificationToken}";
            await _emailService.SendEmailAsync(email, "Email verification", notificationMessage);
        }

        public async Task NotifyUser(EmailVerification message)
        {
            var user = await _db.Set<User>().FirstOrDefaultAsync(x => x.Email == message.Token.Email);
            var notification = new Notification
            {
                UserId = user.UserId,
                PublisherId = user.UserId,
                Message = "Verification token has been sent to you email. Please verify it"
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
