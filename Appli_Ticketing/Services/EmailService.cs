using System;
using System.Configuration;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Appli_Ticketing.Services
{
    public static class EmailService
    {

        private static readonly string Host = ConfigurationManager.AppSettings["SmtpHost"];
        private static readonly int Port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        private static readonly bool EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
        private static readonly string User = ConfigurationManager.AppSettings["SmtpUser"];
        private static readonly string Pass = ConfigurationManager.AppSettings["SmtpPass"];
        private static readonly string From = ConfigurationManager.AppSettings["FromAddress"];

        public static async Task SendAsync(string to, string subject, string body, params string[] attachments)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    if (System.IO.File.Exists(file))
                        builder.Attachments.Add(file);
                }
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(Host, Port, SecureSocketOptions.StartTls);
            if (!string.IsNullOrWhiteSpace(User))
                await client.AuthenticateAsync(User, Pass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}