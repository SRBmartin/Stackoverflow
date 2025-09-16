using MediatR;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;
using StackoverflowService.Application.DTOs.Users;
using StackoverflowService.Domain.Enums;
using StackoverflowService.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Features.Questions.UpdateQuestion
{
    public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result<QuestionDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IVoteRepository _voteRepository;

        public UpdateQuestionCommandHandler(
            IQuestionRepository questionRepository,
            IUserRepository userRepository,
            IAnswerRepository answerRepository,
            IVoteRepository voteRepository)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _answerRepository = answerRepository;
            _voteRepository = voteRepository;
        }

        public async Task<Result<QuestionDto>> Handle(UpdateQuestionCommand command, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetAsync(command.UserId, command.QuestionId, cancellationToken);
            if (question is null)
            {
                return Result.Fail<QuestionDto>(Error.NotFound("Questions.NotFoundOrForbidden", "Question not found or you do not have permission to edit it."));
            }

            if (question.IsClosed)
            {
                return Result.Fail<QuestionDto>(Error.Conflict("Questions.Closed", "Question is closed and cannot be edited."));
            }

            question.Edit(command.Title, command.Description);

            await _questionRepository.UpdateAsync(question, cancellationToken);

            var answers = await _answerRepository.ListByQuestionAsync(question.Id, take: 0, cancellationToken);
            var voteScore = 0;
            foreach (var a in answers)
            {
                var votes = await _voteRepository.ListByAnswerAsync(a.Id, take: 0, cancellationToken);
                foreach (var v in votes)
                {
                    if (v.Type == VoteType.Up) voteScore += 1;
                    else if (v.Type == VoteType.Down) voteScore -= 1;
                }
            }

            var user = await _userRepository.GetAsync(question.UserId, cancellationToken);

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
