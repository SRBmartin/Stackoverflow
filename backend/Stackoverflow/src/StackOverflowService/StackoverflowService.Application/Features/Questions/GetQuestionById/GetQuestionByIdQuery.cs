using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Questions;

namespace StackoverflowService.Application.Features.Questions.GetQuestionById
{
    public class GetQuestionByIdQuery : IRequest<Result<QuestionDto>>
    {
        public string QuestionId { get; }

        public GetQuestionByIdQuery(string questionId)
        {
            QuestionId = questionId;
        }

    }
}
