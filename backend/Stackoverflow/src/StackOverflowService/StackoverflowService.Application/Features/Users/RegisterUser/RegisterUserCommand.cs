using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Auth;

namespace StackoverflowService.Application.Features.Users.RegisterUser
{
    public sealed class RegisterUserCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Name { get; }
        public string Lastname { get; }
        public string Email { get; }
        public string Password { get; }
        public string Gender { get; }
        public string State { get; }
        public string City { get; }
        public string Address { get; }

        public RegisterUserCommand(string name, string lastname, string email, string password,
                                   string gender, string state, string city, string address)
        {
            Name = name; Lastname = lastname; Email = email; Password = password;
            Gender = gender; State = state; City = city; Address = address;
        }

    }
}
