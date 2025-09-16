using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.File;

namespace StackoverflowService.Application.Features.Users.GetUserPhoto
{
    public class GetUserPhotoQuery : IRequest<Result<FileDownloadDto>>
    {
        public string UserId { get; }

        public GetUserPhotoQuery(string userId)
        {
            UserId = userId;
        }

    }
}
