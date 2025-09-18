using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Users;

namespace StackoverflowService.Application.Features.Users.GetUserProfile
{
    public sealed class GetUserProfileQuery : IRequest<Result<UserDto>>
    {
        public string UserId { get; }

        public GetUserProfileQuery(string userId)
        {
            UserId = userId;
        }
    }
}
