using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;

namespace StackoverflowService.Application.Features.Questions.CreateQuestion
{
    public class CreateQuestionCommand : IRequest<Result<QuestionDto>>
    {
        public string UserId { get; }
        public string Title { get; }
        public string Description { get; }

        public CreateQuestionCommand(string userId, string title, string description)
        {
            UserId = userId;
            Title = title;
            Description = description;
        }

    }
}
