using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Answers;

namespace StackoverflowService.Application.Features.Answers.CreateAnswer
{
    public class CreateAnswerCommand : IRequest<Result<AnswerDto>>
    {
        public string UserId { get; }
        public string QuestionId { get; }
        public string Text { get; }

        public CreateAnswerCommand(string userId, string questionId, string text)
        {
            UserId = userId;
            QuestionId = questionId;
            Text = text;
        }

    }
}
