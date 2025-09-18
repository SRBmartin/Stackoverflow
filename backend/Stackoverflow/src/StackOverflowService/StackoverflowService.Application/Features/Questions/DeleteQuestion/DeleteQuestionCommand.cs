using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;

namespace StackoverflowService.Application.Features.Questions.DeleteQuestion
{
    public class DeleteQuestionCommand : IRequest<Result<bool>>
    {
        public string UserId { get; }
        public string QuestionId { get; }

        public DeleteQuestionCommand(string userId, string questionId)
        {
            UserId = userId;
            QuestionId = questionId;
        }

    }
}
