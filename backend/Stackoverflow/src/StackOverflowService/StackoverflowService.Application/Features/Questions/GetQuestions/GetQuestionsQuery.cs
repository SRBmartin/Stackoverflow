using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Common;
using StackoverflowService.Application.DTOs.Questions;
using StackoverflowService.Application.Features.Questions.GetQuestions.Enums;

#nullable enable

namespace StackoverflowService.Application.Features.Questions.GetQuestions
{
    public class GetQuestionsQuery : IRequest<Result<PagedResult<QuestionDto>>>
    {
        public int Page { get; }
        public string? TitleStartsWith { get; } = string.Empty;
        public QuestionsSortBy SortBy { get; }
        public SortDirection Direction { get; }

        public GetQuestionsQuery(int page, string? titleStartsWith, QuestionsSortBy sortBy, SortDirection direction)
        {
            Page = page <= 0 ? 1 : page;
            TitleStartsWith = titleStartsWith;
            SortBy = sortBy;
            Direction = direction;
        }

    }
}
