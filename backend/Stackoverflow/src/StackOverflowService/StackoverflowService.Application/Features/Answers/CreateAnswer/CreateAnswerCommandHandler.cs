using MediatR;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Answers;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Domain.Repositories;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace StackoverflowService.Application.Features.Answers.CreateAnswer
{
    public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, Result<AnswerDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public CreateAnswerCommandHandler(
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public async Task<Result<AnswerDto>> Handle(CreateAnswerCommand command, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetByIdAsync(command.QuestionId, cancellationToken);
            if (question is null || question.IsDeleted)
                return Result.Fail<AnswerDto>(Error.NotFound("Questions.NotFound", "Question not found."));

            if (question.IsClosed)
                return Result.Fail<AnswerDto>(Error.Conflict("Questions.Closed", "Question is closed and cannot be answered."));

            var answerId = Guid.NewGuid().ToString("N");
            var answer = new Answer(
                id: answerId,
                questionId: question.Id,
                userId: command.UserId,
                text: command.Text,
                created: DateTimeOffset.UtcNow
            );

            await _answerRepository.AddAsync(answer, cancellationToken);

            var dto = new AnswerDto
            {
                Id = answer.Id,
                QuestionId = answer.QuestionId,
                UserId = answer.UserId,
                Text = answer.Text,
                CreationDate = answer.CreationDate,
                IsFinal = answer.IsFinal,
                IsDeleted = answer.IsDeleted,
                UpVotes = 0,
                DownVotes = 0,
                VoteScore = 0
            };

            return Result.Ok(dto);
        }

    }
}
