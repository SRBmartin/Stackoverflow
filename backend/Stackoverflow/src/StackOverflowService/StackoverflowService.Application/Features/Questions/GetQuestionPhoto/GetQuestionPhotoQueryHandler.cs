using MediatR;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.File;
using StackoverflowService.Domain.Repositories;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Application.Common;
using System.IO;

namespace StackoverflowService.Application.Features.Questions.GetQuestionPhoto
{
    public class GetQuestionPhotoQueryHandler : IRequestHandler<GetQuestionPhotoQuery, Result<FileDownloadDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IPhotoReader _photoReader;

        public GetQuestionPhotoQueryHandler(
            IQuestionRepository questionRepository,
            IPhotoReader photoReader)
        {
            _questionRepository = questionRepository;
            _photoReader = photoReader;
        }

        public async Task<Result<FileDownloadDto>> Handle(GetQuestionPhotoQuery query, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetByIdAsync(query.QuestionId, cancellationToken);
            if (question is null || question.IsDeleted)
            {
                return Result.Fail<FileDownloadDto>(Error.NotFound("Questions.NotFound", "Question not found."));
            }

            var container = question.Photo?.Container;
            var blobName = question.Photo?.BlobName;

            if (string.IsNullOrWhiteSpace(container) || string.IsNullOrWhiteSpace(blobName))
            {
                return Result.Fail<FileDownloadDto>(Error.NotFound("Questions.PhotoNotFound", "Question does not have a photo."));
            }

            try
            {
                var file = await _photoReader.DownloadAsync(container, blobName, cancellationToken);

                return Result.Ok(file);
            }
            catch (FileNotFoundException)
            {
                return Result.Fail<FileDownloadDto>(Error.NotFound("Questions.PhotoMissingInStorage", "Stored photo reference not found in blob storage."));
            }

        }

    }
}
