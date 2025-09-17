using FluentValidation;

namespace StackoverflowService.Application.Features.Questions.GetQuestions
{
    public class GetQuestionsQueryValidator : AbstractValidator<GetQuestionsQuery>
    {
        public GetQuestionsQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.TitleStartsWith).MaximumLength(200);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.Direction).IsInEnum();
        }
    }
}
