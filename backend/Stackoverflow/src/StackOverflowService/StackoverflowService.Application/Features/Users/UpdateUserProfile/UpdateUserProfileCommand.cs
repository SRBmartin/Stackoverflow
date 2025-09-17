using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Users;

namespace StackoverflowService.Application.Features.Users.UpdateUserProfile
{
    public sealed class UpdateUserProfileCommand : IRequest<Result<UserDto>>
    {
        public string UserId { get; }
        public string Name { get; }
        public string Lastname { get; }
        public string State { get; }
        public string City { get; }
        public string Address { get; }

        public UpdateUserProfileCommand(string userId, string name, string lastname, string state, string city, string address)
        {
            UserId = userId;
            Name = name;
            Lastname = lastname;
            State = state;
            City = city;
            Address = address;
        }
    }
}
