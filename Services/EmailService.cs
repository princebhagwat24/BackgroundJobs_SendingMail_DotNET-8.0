using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailBackgroundJobDemo.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.mailtrap.io";
        private readonly string _smtpUser = "5bca2073ad10dc@sandbox.mailtrap.io";
        private readonly string _smtpPassword = "cbc616422bd5f3";
        private readonly int _smtpPort = 2525;

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
