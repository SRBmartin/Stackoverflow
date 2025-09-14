using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.File;
using StackoverflowService.Application.DTOs.Users;

namespace StackoverflowService.Application.Features.Users.SetUserPhoto
{
    public class SetUserPhotoCommand : IRequest<Result<UserDto>>
    {
        public string UserId { get; set; } = default!;
        public FileUploadDto File { get; set; } = default!;
    }
}
