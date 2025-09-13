using MediatR;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Auth;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Domain.Enums;
using StackoverflowService.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace StackoverflowService.Application.Features.Users.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IIdentityService _identityService;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IIdentityService identityService
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _identityService = identityService;
        }

        public async Task<Result<AuthResponseDto?>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var email = command.Email.Trim();

            if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            {
                return Result.Fail<AuthResponseDto>(Error.Conflict("User.EmailInUse", "User is already registered with this email address."))!;
            }

            var userId = Guid.NewGuid().ToString();
            var passwordHash = _passwordHasher.Hash(command.Password);
            var gender = ParseGender(command.Gender);

            var user = new User(
                id: userId,
                name: command.Name.Trim(),
                lastname: command.Lastname.Trim(),
                email: email,
                passwordHash: passwordHash,
                gender: gender,
                state: command.State?.Trim() ?? "",
                city: command.City?.Trim() ?? "",
                address: command.Address?.Trim() ?? "",
                photo: null,
                created: DateTimeOffset.UtcNow);

            await _userRepository.AddAsync(user, cancellationToken);

            var response = _identityService.CreateAccessToken(user);

            return Result.Ok(response)!;
        }

        private static Gender ParseGender(string? s)
        {
            switch ((s ?? "").Trim().ToUpperInvariant())
            {
                case "M": return Gender.Male;
                case "F": return Gender.Female;
                default: return Gender.Male;
            }
        }

        private static string ToGenderString(Gender g) =>
            g == Gender.Male ? "M" : "F";

    }
}
