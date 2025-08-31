using eBooks.Database.Models;
using eBooks.Database;
using eBooks.Models.Messages;
using eBooks.Subscriber.Services;
using eBooks.Subscriber.Interfaces;

namespace eBooks.Subscriber.MessageHandlers
{
    public class QuestionAnsweredHandler : BaseMessageHandler<QuestionAnswered>, IMessageHandler<QuestionAnswered>
    {
        public QuestionAnsweredHandler(EBooksContext db, EmailService emailService)
            : base(db, emailService)
        {
        }

        public async Task SendEmail(QuestionAnswered message)
        {
            var email = message.Question.User.Email;
            var notificationMessage = $"Your question \"{message.Question.Question1}\" has been answered.<br>Answer: \"{message.Question.Answer}\"";
            Console.WriteLine($"Sending email to: {email}");
            await _emailService.SendEmailAsync(email, "Question answered", notificationMessage);
        }

        public async Task NotifyUser(QuestionAnswered message)
        {
            var notificationMessage = $"Your question \"{message.Question.Question1}\" has been answered. Answer: \"{message.Question.Answer}\"";
            Console.WriteLine($"Sending notification to user: {message.Question.User.UserId}");
            var notification = new Notification
            {
                UserId = message.Question.User.UserId,
                Message = notificationMessage
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
