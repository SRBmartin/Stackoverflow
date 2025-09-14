using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Auth;

namespace StackoverflowService.Application.Features.Users.Login
{
    public class LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; }
        public string Password { get; }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

    }
}
