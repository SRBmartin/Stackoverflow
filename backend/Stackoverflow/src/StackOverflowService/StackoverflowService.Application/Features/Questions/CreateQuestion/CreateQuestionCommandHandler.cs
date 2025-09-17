using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;
using StackoverflowService.Domain.Repositories;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Application.Common;
using System;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Application.DTOs.Users;

namespace StackoverflowService.Application.Features.Questions.CreateQuestion
{
    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<QuestionDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;

        public CreateQuestionCommandHandler(
            IQuestionRepository questionRepository,
            IUserRepository userRepository)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<QuestionDto>> Handle(CreateQuestionCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
            if (user is null)
            {
                return Result.Fail<QuestionDto>(Error.NotFound("Users.NotFound", "User not found."));
            }

            var questionId = Guid.NewGuid().ToString("N");

            var question = new Question(
                questionId,
                user.Id,
                command.Title,
                command.Description,
                null,
                DateTimeOffset.UtcNow
            );

            await _questionRepository.AddAsync(question, cancellationToken);

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
