using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;

#nullable enable

namespace StackoverflowService.Application.Features.Questions.UpdateQuestion
{
    public class UpdateQuestionCommand : IRequest<Result<QuestionDto>>
    {
        public string UserId { get; }
        public string QuestionId { get; }
        public string Title { get; }
        public string Description { get; }

        public UpdateQuestionCommand(string userId, string questionId, string title, string description)
        {
            UserId = userId;
            QuestionId = questionId;
            Title = title;
            Description = description;
        }

    }
}
