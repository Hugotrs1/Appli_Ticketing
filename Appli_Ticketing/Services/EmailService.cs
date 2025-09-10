using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Appli_Ticketing.Services
{
    public static class EmailService
    {

        private static readonly string Host = ConfigurationManager.AppSettings["SmtpHost"] ?? throw new InvalidOperationException("SmtpHost n'est pas configuré.");
        private static readonly int Port = int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out var port) ? port : throw new InvalidOperationException("SmtpPort n'est pas configuré ou n'est pas un entier valide.");
        private static readonly bool EnableSsl = bool.TryParse(ConfigurationManager.AppSettings["EnableSsl"], out var enableSsl) ? enableSsl : throw new InvalidOperationException("EnableSsl n'est pas configuré ou n'est pas un booléen valide.");
        private static readonly string User = ConfigurationManager.AppSettings["SmtpUser"] ?? string.Empty;
        private static readonly string Pass = ConfigurationManager.AppSettings["SmtpPass"] ?? string.Empty;
        private static readonly string From = ConfigurationManager.AppSettings["FromAddress"] ?? throw new InvalidOperationException("FromAddress n'est pas configuré.");

        public static async Task SendAsync(string to, string subject, string bodyHtml, string imagePath = null)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            string contentId = "CriticiteImage";

            var builder = new BodyBuilder();

            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                var image = builder.LinkedResources.Add(imagePath);
                image.ContentId = contentId;
                image.ContentType.MediaType = "image";
                image.ContentType.Name = Path.GetFileName(imagePath);
            }

            builder.HtmlBody = bodyHtml;

            
            var multipart = new Multipart("related")
    {
        builder.ToMessageBody() 
    };

            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(Host, Port, SecureSocketOptions.StartTls);
            if (!string.IsNullOrWhiteSpace(User))
                await client.AuthenticateAsync(User, Pass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}