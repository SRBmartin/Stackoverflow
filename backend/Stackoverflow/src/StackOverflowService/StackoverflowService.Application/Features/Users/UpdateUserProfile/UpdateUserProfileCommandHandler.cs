using MediatR;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Users;
using StackoverflowService.Application.Features.Users.GetUserProfile;
using StackoverflowService.Domain.Enums;
using StackoverflowService.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Features.Users.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(command.UserId, cancellationToken);

            if (user == null)
            {
                return Result.Fail<UserDto>(Error.NotFound("User.NotFound", "User not found."));
            }

            if (user.Name == command.Name?.Trim() &&
                user.Lastname == command.Lastname?.Trim() &&
                user.State == (command.State?.Trim() ?? "") &&
                user.City == (command.City?.Trim() ?? "") &&
                user.Address == (command.Address?.Trim() ?? ""))
            {
                return Result.Fail<UserDto>(Error.Failure("User.NoChangesDetected", "No changes detected."));
            }

            user.UpdateProfile(
                name: command.Name,
                lastname: command.Lastname,
                state: command.State,
                city: command.City,
                address: command.Address
            );

            await _userRepository.UpdateAsync(user, cancellationToken);

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email,
                Gender = user.Gender == Gender.Male ? "M" : "F",
                State = user.State,
                City = user.City,
                Address = user.Address,
                PhotoBlobName = user.Photo?.BlobName,
                PhotoContainer = user.Photo?.Container,
                CreationDate = user.CreationDate
            };

            return Result.Ok(userDto);
        }
    }
}
