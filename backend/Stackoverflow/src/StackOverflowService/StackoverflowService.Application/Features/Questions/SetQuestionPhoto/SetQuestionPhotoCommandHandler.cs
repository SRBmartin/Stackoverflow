using MediatR;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;
using StackoverflowService.Domain.Repositories;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Application.Common;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Application.DTOs.Users;

#nullable enable

namespace StackoverflowService.Application.Features.Questions.SetQuestionPhoto
{
    public class SetQuestionPhotoCommandHandler : IRequestHandler<SetQuestionPhotoCommand, Result<QuestionDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQuestionPhotoStorage _photoStorage;

        public SetQuestionPhotoCommandHandler(
            IQuestionRepository questionRepository,
            IUserRepository userRepository,
            IQuestionPhotoStorage photoStorage)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _photoStorage = photoStorage;
        }

        public async Task<Result<QuestionDto>> Handle(SetQuestionPhotoCommand command, CancellationToken cancellationToken)
        {
            Question? question = await _questionRepository.GetAsync(command.UserId, command.QuestionId, cancellationToken);
            if (question is null)
            {
                return Result.Fail<QuestionDto>(Error.NotFound("Questions.NotFoundOrForbidden", "Question not found or you do not have permission to modify it."));
            }

            var photoRef = await _photoStorage.UploadQuestionPhotoAsync(
                userId: command.UserId,
                questionId: command.QuestionId,
                content: command.File.Content,
                contentType: command.File.ContentType,
                fileName: command.File.FileName,
                cancellationToken: cancellationToken);

            question.SetPhoto(photoRef);

            await _questionRepository.UpdateAsync(question, cancellationToken);

            var user = await _userRepository.GetAsync(command.UserId, cancellationToken);

            var dto = new QuestionDto
            {
                Id = question.Id,
                UserId = question.UserId,
                Title = question.Title,
                Description = question.Description,
                PhotoBlobName = question.Photo?.BlobName,
                PhotoContainer = question.Photo?.Container,
                CreationDate = question.CreationDate,
                IsClosed = question.IsClosed,
                IsDeleted = question.IsDeleted,
                User = new UserPreviewDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Lastname = user.Lastname,
                        Email = user.Email,
                        PhotoBlobName = user.Photo?.BlobName,
                        PhotoContainer = user.Photo?.Container
                    }
            };

            return Result.Ok(dto);
        }

    }
}
