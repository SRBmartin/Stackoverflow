namespace MailService.Application.Contracts.Services
{
    public interface IMailSenderService
    {
        Task<bool> SendMailAsync(string to, string subject, string body);
    }
}
