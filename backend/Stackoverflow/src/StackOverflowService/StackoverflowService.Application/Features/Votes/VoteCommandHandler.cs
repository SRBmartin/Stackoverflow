using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Domain.Repositories;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Application.Common;
using StackoverflowService.Domain.Entities;
using System;
using StackoverflowService.Domain.Enums;

namespace StackoverflowService.Application.Features.Votes
{
    public class VoteCommandHandler : IRequestHandler<VoteCommand, Result<bool>>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IVoteRepository _voteRepository;

        public VoteCommandHandler(
            IAnswerRepository answerRepository,
            IQuestionRepository questionRepository,
            IVoteRepository voteRepository)
        {
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
            _voteRepository = voteRepository;
        }

        public async Task<Result<bool>> Handle(VoteCommand command, CancellationToken ct)
        {
            VoteType desiredType;
            if (!TryParseVoteType(command.Type, out desiredType))
                return Result.Fail<bool>(Error.Validation("Votes.TypeInvalid", "Invalid vote 'type'. Use '+', '-', 'up', or 'down'."));

            var question = await _questionRepository.GetByIdAsync(command.QuestionId, ct);
            if (question is null || question.IsDeleted)
                return Result.Fail<bool>(Error.NotFound("Questions.NotFound", "Question not found."));

            if (question.IsClosed)
                return Result.Fail<bool>(Error.Conflict("Question.IsClosed", "Question is closed."));

            if (command.AnswerId is null)
            {
                if (question.UserId == command.UserId)
                    return Result.Fail<bool>(Error.Forbidden("Votes.OwnContent", "You cannot vote on your own content."));

                var existing = await _voteRepository.GetUserVoteForQuestionAsync(command.QuestionId, command.UserId, ct);

                if (existing == null)
                {
                    var v = Vote.ForQuestion(
                        id: Guid.NewGuid().ToString("N"),
                        questionId: command.QuestionId,
                        userId: command.UserId,
                        type: desiredType,
                        created: DateTimeOffset.UtcNow
                    );
                    await _voteRepository.AddAsync(v, ct);
                }
                else if (existing.Type == desiredType)
                {
                    await _voteRepository.DeleteAsync(existing, ct);
                }
                else
                {
                    existing.Switch(desiredType);
                    await _voteRepository.UpsertAsync(existing, ct);
                }

                return Result.Ok(true);
            }
            else
            {
                var answer = await _answerRepository.GetAsync(command.QuestionId, command.AnswerId, ct);
                if (answer is null || answer.IsDeleted)
                    return Result.Fail<bool>(Error.NotFound("Answers.NotFound", "Answer not found."));

                if (answer.UserId == command.UserId || question.UserId == command.UserId)
                    return Result.Fail<bool>(Error.Forbidden("Votes.OwnContent", "You cannot vote on your own content."));

                var existing = await _voteRepository.GetUserVoteForAnswerAsync(answer.Id, command.UserId, ct);

                if (existing == null)
                {
                    var v = Vote.ForAnswer(
                        id: Guid.NewGuid().ToString("N"),
                        answerId: answer.Id,
                        userId: command.UserId,
                        type: desiredType,
                        created: DateTimeOffset.UtcNow
                    );
                    await _voteRepository.AddAsync(v, ct);
                }
                else if (existing.Type == desiredType)
                {
                    await _voteRepository.DeleteAsync(existing, ct);
                }
                else
                {
                    existing.Switch(desiredType);
                    await _voteRepository.UpsertAsync(existing, ct);
                }

            }

            return Result.Ok(true);
        }

        #region Helpers
        private static bool TryParseVoteType(string typeRaw, out VoteType t)
        {
            t = VoteType.Up;
            if (string.IsNullOrWhiteSpace(typeRaw)) return false;
            var s = typeRaw.Trim().ToLowerInvariant();
            if (s == "+" || s == "up") { t = VoteType.Up; return true; }
            if (s == "-" || s == "down") { t = VoteType.Down; return true; }
            return false;
        }
        #endregion

    }

}
