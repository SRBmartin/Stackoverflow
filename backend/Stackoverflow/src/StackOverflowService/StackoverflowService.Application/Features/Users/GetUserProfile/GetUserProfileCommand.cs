using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Features.Users.GetUserProfile
{
    public sealed class GetUserProfileCommand : IRequest<Result<UserDto>>
    {
        public string UserId { get; }

        public GetUserProfileCommand(string userId)
        {
            UserId = userId;
        }
    }
}
