using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.File;
using StackoverflowService.Application.DTOs.Questions;

namespace StackoverflowService.Application.Features.Questions.SetQuestionPhoto
{
    public class SetQuestionPhotoCommand : IRequest<Result<QuestionDto>>
    {
        public string UserId { get; set; } = default!;
        public string QuestionId { get; set; } = default!;
        public FileUploadDto File { get; set; } = default!;
    }
}
