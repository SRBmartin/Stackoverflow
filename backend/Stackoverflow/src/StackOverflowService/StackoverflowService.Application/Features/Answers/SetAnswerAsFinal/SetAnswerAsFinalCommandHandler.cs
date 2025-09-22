using MediatR;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.Features.Answers.Events.AnswerMarkerFinal;
using StackoverflowService.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Features.Answers.SetAnswerAsFinal
{
    public class SetAnswerAsFinalCommandHandler : IRequestHandler<SetAnswerAsFinalCommand, Result<bool>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IMediator _mediator;

        public SetAnswerAsFinalCommandHandler(
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository,
            IMediator mediator)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _mediator = mediator;
        }

        public async Task<Result<bool>> Handle(SetAnswerAsFinalCommand command, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetByIdAsync(command.QuestionId, cancellationToken);
            if (question is null || question.IsDeleted)
                return Result.Fail<bool>(Error.NotFound("Questions.NotFound", "Question not found."));

            if (!string.Equals(question.UserId, command.UserId, System.StringComparison.Ordinal))
                return Result.Fail<bool>(Error.Forbidden("Questions.Forbidden", "You are not allowed to mark final for this question."));

            var answer = await _answerRepository.GetAsync(question.Id, command.AnswerId, cancellationToken);
            if (answer is null || answer.IsDeleted)
                return Result.Fail<bool>(Error.NotFound("Answers.NotFound", "Answer not found."));

            var existingFinal = await _answerRepository.GetFinalByQuestionAsync(question.Id, cancellationToken);
            if (existingFinal != null)
            {
                if (existingFinal.Id == answer.Id)
                    return Result.Fail<bool>(Error.Conflict("Answers.AlreadyFinal", "Answer is already set as final."));
                return Result.Fail<bool>(Error.Conflict("Answers.AnotherFinalExists", "Another answer is already set as final."));
            }

            answer.MarkFinal();
            await _answerRepository.UpdateAsync(answer, cancellationToken);

            question.Close();
            await _questionRepository.UpdateAsync(question, cancellationToken);

            var notification = new AnswerMarkedFinalNotification(command.QuestionId);
            await _mediator.Publish(notification, cancellationToken);

            return Result.Ok(true);
        }

    }
}
