//using MailKit.Net.Smtp;
//using MailKit.Security;
//using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    public class EmailService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mizuho.eps@gmail.com", "mizuhoeps2019"),
                EnableSsl = true,
                Port = 465
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("mizuho.eps@gmail.com")
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject ?? "[EPS]";
            mailMessage.Body = htmlMessage;
            return client.SendMailAsync(mailMessage);
        }
    }
}
