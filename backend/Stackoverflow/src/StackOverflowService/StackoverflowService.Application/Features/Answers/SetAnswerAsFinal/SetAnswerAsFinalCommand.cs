using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;

namespace StackoverflowService.Application.Features.Answers.SetAnswerAsFinal
{
    public class SetAnswerAsFinalCommand : IRequest<Result<bool>>
    {
        public string UserId { get; }
        public string QuestionId { get; }
        public string AnswerId { get; }

        public SetAnswerAsFinalCommand(string userId, string questionId, string answerId)
        {
            UserId = userId;
            QuestionId = questionId;
            AnswerId = answerId;
        }

    }
}
