using MediatR;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Users;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Domain.Enums;
using StackoverflowService.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace StackoverflowService.Application.Features.Users.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<UserDto?>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var email = command.Email.Trim();

            if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            {
                return Result.Fail<UserDto>(Error.Conflict("User.EmailInUse", "User is already registered with this email address."))!;
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

            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email,
                Gender = ToGenderString(user.Gender),
                State = user.State,
                City = user.City,
                Address = user.Address,
                PhotoBlobName = user.Photo?.BlobName,
                PhotoContainer = user.Photo?.Container,
                CreationDate = user.CreationDate
            };

            return Result.Ok(dto)!;
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
