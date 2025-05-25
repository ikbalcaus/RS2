using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace eBooks.Subscriber.Services
{
    public class EmailService
    {
        protected IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MailMessage();
            email.From = new MailAddress(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderName"]);
            email.To.Add(toEmail);
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;
            using var smtp = new SmtpClient(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]))
            {
                Credentials = new NetworkCredential(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]),
                EnableSsl = true
            };
            await smtp.SendMailAsync(email);
        }
    }
}
