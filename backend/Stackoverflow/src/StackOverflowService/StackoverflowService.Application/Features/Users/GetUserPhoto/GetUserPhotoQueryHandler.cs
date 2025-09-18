using MediatR;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.File;
using StackoverflowService.Domain.Repositories;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace StackoverflowService.Application.Features.Users.GetUserPhoto
{
    public class GetUserPhotoQueryHandler : IRequestHandler<GetUserPhotoQuery, Result<FileDownloadDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoReader _photoReader;

        public GetUserPhotoQueryHandler(IUserRepository userRepository, IPhotoReader photoReader)
        {
            _userRepository = userRepository;
            _photoReader = photoReader;
        }

        public async Task<Result<FileDownloadDto>> Handle(GetUserPhotoQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(query.UserId, cancellationToken);
            if (user is null)
            {
                return Result.Fail<FileDownloadDto>(Error.NotFound("Users.NotFound", "User not found."));
            }

            var container = user.Photo?.Container;
            var blobName = user.Photo?.BlobName;

            if (string.IsNullOrWhiteSpace(container) || string.IsNullOrWhiteSpace(blobName))
            {
                return Result.Fail<FileDownloadDto>(Error.NotFound("Users.PhotoNotFound", "User does not have a photo."));
            }

            try
            {
                var file = await _photoReader.DownloadAsync(container, blobName, cancellationToken);
                return Result.Ok(file);
            }
            catch (FileNotFoundException)
            {
                return Result.Fail<FileDownloadDto>(Error.NotFound("Users.PhotoMissingInStorage", "Stored photo reference not found in blob storage."));
            }

        }

    }
}
