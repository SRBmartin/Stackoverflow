using System.Security.Claims;

namespace StackoverflowService.Application.Abstractions
{
    public interface IIdentityValidator
    {
        bool TryValidate(string token, out ClaimsPrincipal principal, out string error);
    }
}
