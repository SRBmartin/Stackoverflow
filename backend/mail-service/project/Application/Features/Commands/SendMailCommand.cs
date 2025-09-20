using MailService.Application.Contracts.Services;
using MediatR;

namespace MailService.Application.Features.Commands;

public class SendMailCommandRequest : IRequest<bool>
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}

public class SendMailCommandRequestHandler : IRequestHandler<SendMailCommandRequest, bool>
{
    private readonly IMailSenderService _mailSenderService;

    public SendMailCommandRequestHandler(IMailSenderService mailSenderService)
    {
        _mailSenderService = mailSenderService;
    }
    public async Task<bool> Handle(SendMailCommandRequest request, CancellationToken cancellationToken = default)
    {
        return await _mailSenderService.SendMailAsync(request.To, request.Subject, request.Body);
    }
}
