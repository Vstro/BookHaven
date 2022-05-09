using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System;
using BookHaven.Models;

namespace BookHaven.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(String email, String subject, String message, Base64File[] attachments = null)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("BookHaven Administration", "bookhaven.noreply@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            if (attachments == null)
            {
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };
            }
            else
            {
                var builder = new BodyBuilder();
                builder.HtmlBody = message;
                foreach (var attachment in attachments)
                {
                    builder.Attachments.Add(attachment.FileName, Convert.FromBase64String(attachment.Data));
                }
                emailMessage.Body = builder.ToMessageBody();
            }

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("bookhaven.noreply@gmail.com", "v8DBUX9G");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}