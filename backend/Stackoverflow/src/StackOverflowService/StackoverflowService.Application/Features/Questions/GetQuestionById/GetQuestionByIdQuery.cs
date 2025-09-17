using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;

namespace StackoverflowService.Application.Features.Questions.GetQuestionById
{
    public class GetQuestionByIdQuery : IRequest<Result<QuestionDto>>
    {
        public string QuestionId { get; }
        public string CurrentUserId { get; }

        public GetQuestionByIdQuery(string questionId, string currentUserId)
        {
            QuestionId = questionId;
            CurrentUserId = currentUserId;
        }

    }
}
