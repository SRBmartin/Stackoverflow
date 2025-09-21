using MailService.Infrastructure.Services.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace MailService.Controllers.Middleware;

public class BearerAuthAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _expectedToken;

    public BearerAuthAttribute(IOptions<AuthSettings> authOptions)
    {
        _expectedToken = authOptions.Value.Token;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authHeader.ToString();

        if (token != _expectedToken)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }

}
