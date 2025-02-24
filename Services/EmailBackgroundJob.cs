using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading;

namespace EmailBackgroundJobDemo.Services
{
    public class EmailBackgroundJob
    {
        private readonly IEmailService _emailService;

        public EmailBackgroundJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void SendEmailJob(string to, string subject, string body)
        {
            const int maxRetries = 3;
            int attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Mailtrap Sender", "e5dc59d1352d8a@sandbox.mailtrap.io"));
                    message.To.Add(new MailboxAddress("", to));
                    message.Subject = subject;
                    message.Body = new TextPart("html") { Text = body };

                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Connect("sandbox.smtp.mailtrap.io", 2525, MailKit.Security.SecureSocketOptions.StartTls);
                        smtpClient.Authenticate("e5dc59d1352d8a", "8e96e177cbe36b");

                        smtpClient.Send(message);
                        smtpClient.Disconnect(true);
                    }

                    Console.WriteLine("Email sent successfully.");
                    break;
                }
                catch (MailKit.Net.Smtp.SmtpProtocolException ex)
                {
                    Console.WriteLine("SMTP Error: " + ex.Message);

                    if (ex.Message.Contains("Too many failed login attempts"))
                    {
                        attempt++;
                        Console.WriteLine($"Retrying... Attempt {attempt} of {maxRetries}");
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending email: " + ex.Message);
                    throw;
                }
            }
        }
    }
}
