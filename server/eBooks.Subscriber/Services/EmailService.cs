using System.Net.Mail;
using System.Net;

namespace eBooks.Subscriber.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MailMessage();
            email.From = new MailAddress(Environment.GetEnvironmentVariable("_smtpSenderEmail"), Environment.GetEnvironmentVariable("_smtpSenderName"));
            email.To.Add(toEmail);
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;
            using var smtp = new SmtpClient(Environment.GetEnvironmentVariable("_smtpServer"), int.Parse(Environment.GetEnvironmentVariable("_smtpPort")))
            {
                Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("_smtpSenderEmail"), Environment.GetEnvironmentVariable("_smtpSenderPassword")),
                EnableSsl = true
            };
            await smtp.SendMailAsync(email);
        }
    }
}
