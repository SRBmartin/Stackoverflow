using MediatR;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Users;
using StackoverflowService.Domain.Enums;
using StackoverflowService.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Features.Users.SetUserPhoto
{
    public class SetUserPhotoCommandHandler : IRequestHandler<SetUserPhotoCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoStorage _photoStorage;

        public SetUserPhotoCommandHandler(
            IUserRepository userRepository,
            IPhotoStorage photoStorage
        )
        {
            _userRepository = userRepository;
            _photoStorage = photoStorage;
        }

        public async Task<Result<UserDto>> Handle(SetUserPhotoCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                return Result.Fail<UserDto>(Error.NotFound("User.NotFound", $"User '{command.UserId}' not found."));
            }

            var photoRef = await _photoStorage.UploadUserPhotoAsync(
                userId: command.UserId,
                content: command.File.Content,
                contentType: command.File.ContentType,
                fileName: command.File.FileName,
                cancellationToken: cancellationToken);

            user.SetPhoto(photoRef);

            await _userRepository.UpdateAsync(user, cancellationToken);

            var dto = new UserDto
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

            return Result.Ok(dto);
        }
    }
}
