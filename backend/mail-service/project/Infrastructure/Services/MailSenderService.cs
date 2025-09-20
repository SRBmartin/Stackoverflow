using MailService.Application.Contracts.Services;
using MailService.Infrastructure.Services.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace MailService.Infrastructure.Services;

public class MailSenderService : IMailSenderService
{
    private readonly MailSettings _settings;

    public MailSenderService(MailSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> SendMailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            
            await client.ConnectAsync(_settings.SmtpServer, _settings.Port, true);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}
