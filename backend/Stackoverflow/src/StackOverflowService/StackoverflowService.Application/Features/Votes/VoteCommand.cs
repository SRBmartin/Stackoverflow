using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Domain.Enums;

#nullable enable

namespace StackoverflowService.Application.Features.Votes
{
    public class VoteCommand : IRequest<Result<bool>>
    {
        public string UserId { get; }
        public string? AnswerId { get; }
        public string QuestionId { get; }
        public string Type { get; }

        public VoteCommand(string userId, string answerId, string questionId, string type)
        {
            UserId = userId;
            AnswerId = answerId;
            QuestionId = questionId;
            Type = type;
        }

    }
}
