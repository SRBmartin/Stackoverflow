using MediatR;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Features.Users.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(command.UserId, cancellationToken);

            if (user == null)
            {
                return Result.Fail(Error.NotFound("User.NotFound", "User not found."));
            }

            if (user.Name == command.Name &&
                user.Lastname == command.Lastname &&
                user.State == command.State &&
                user.City == command.City &&
                user.Address == command.Address)
            {
                return Result.Fail(Error.Failure("User.NoChangesDetected", "No changes detected."));
            }

            user.GetType().GetProperty("Name")?.SetValue(user, command.Name.Trim());
            user.GetType().GetProperty("Lastname")?.SetValue(user, command.Lastname.Trim());
            user.GetType().GetProperty("State")?.SetValue(user, command.State?.Trim() ?? "");
            user.GetType().GetProperty("City")?.SetValue(user, command.City?.Trim() ?? "");
            user.GetType().GetProperty("Address")?.SetValue(user, command.Address?.Trim() ?? "");

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Result.Ok();
        }
    }
}
