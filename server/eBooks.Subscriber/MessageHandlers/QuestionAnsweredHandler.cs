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
            Console.WriteLine($"Sending email to: {email}");
            await _emailService.SendEmailAsync(email, "Question answered", $"Your question \"{message.Question.Question1}\" is answered. Answer: \"{message.Question.Answer}\"");
        }

        public async Task NotifyUser(QuestionAnswered message)
        {
            Console.WriteLine($"Sending notification to user: {message.Question.UserId}");
            var notification = new Notification
            {
                UserId = message.Question.User.UserId,
                Message = $"Your question \"{message.Question.Question1}\" is answered. Answer: \"{message.Question.Answer}\""
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
