using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.File;

namespace StackoverflowService.Application.Features.Questions.GetQuestionPhoto
{
    public class GetQuestionPhotoQuery : IRequest<Result<FileDownloadDto>>
    {
        public string QuestionId { get; set; }

        public GetQuestionPhotoQuery(string questionId)
        {
            QuestionId = questionId;
        }

    }
}
