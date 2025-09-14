using MediatR;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Auth;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace StackoverflowService.Application.Features.Users.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IIdentityService identityService
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _identityService = identityService;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (user == null)
            {
                return Result.Fail<AuthResponseDto>(Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));
            }

            var verified = _passwordHasher.Verify(command.Password, user.PasswordHash);
            if (!verified)
            {
                return Result.Fail<AuthResponseDto>(Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password"));
            }

            var token = _identityService.CreateAccessToken(user);

            return Result.Ok(token);
        }

    }
}
