using StackoverflowService.Application.DTOs.Auth;
using StackoverflowService.Domain.Entities;

namespace StackoverflowService.Application.Abstractions
{
    public interface IIdentityService
    {
        AuthResponseDto CreateAccessToken(User user);
    }
}
