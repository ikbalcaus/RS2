using eBooks.Models.Messages;
using eBooks.Database;
using eBooks.Subscriber.Services;
using eBooks.Subscriber.Interfaces;
using eBooks.Subscriber.MessageHandlers;
using eBooks.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.MessageHandlers
{
    public class PasswordForgottenHandler : BaseMessageHandler<PasswordForgotten>, IMessageHandler<PasswordForgotten>
    {
        public PasswordForgottenHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(PasswordForgotten message)
        {
            var email = message.Token.Email;
            var notificationMessage = $"Click the following link in order to reset you password: {message.Token.VerificationToken}";
            Console.WriteLine($"Sending email to: {email}");
            await _emailService.SendEmailAsync(email, "Email verification", notificationMessage);
        }

        public async Task NotifyUser(PasswordForgotten message)
        {
            var user = await _db.Set<User>().FirstOrDefaultAsync(x => x.Email == message.Token.Email);
            Console.WriteLine($"Sending notification to user: {user.UserId}");
            var notification = new Notification
            {
                UserId = user.UserId,
                PublisherId = user.UserId,
                Message = "Verification link has been sent to you email"
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
