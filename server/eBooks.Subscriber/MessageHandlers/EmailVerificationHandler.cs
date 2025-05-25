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
            Console.WriteLine($"Sending email to: {message.Token.Email}");
            await _emailService.SendEmailAsync(message.Token.Email, "Email verification", message.Token.VerificationToken);
        }

        public async Task NotifyUser(EmailVerification message)
        {
            var user = await _db.Set<User>().FirstOrDefaultAsync(x => x.Email == message.Token.Email);
            Console.WriteLine($"Sending notification to user: {user.UserId}");
            var notification = new Notification
            {
                UserId = user.UserId,
                PublisherId = user.UserId,
                Message = "Please check your email and verify it"
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
