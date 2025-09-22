using MailService.Application.Features.Commands;
using MailService.Controllers.Middleware;
using MailService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MailService.Controllers;

[ApiController]
[Route("[controller]")]
public class MailController : ControllerBase
{
    private readonly IMediator _mediator;

    public MailController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("send")]
    [ServiceFilter(typeof(BearerAuthAttribute))]
    public async Task<IActionResult> SendMailAsync([FromBody] Mail mail)
    {
        var result = await _mediator.Send(new SendMailCommandRequest
        {
            To = mail.To,
            Subject = mail.Subject,
            Body = mail.Body
        });

        if (result)
            return Ok(result);
        return StatusCode(500, "Failed to send email.");
    }

}
