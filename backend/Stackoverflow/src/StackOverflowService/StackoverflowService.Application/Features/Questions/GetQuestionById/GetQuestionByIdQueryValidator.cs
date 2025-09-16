using FluentValidation;

namespace StackoverflowService.Application.Features.Questions.GetQuestionById
{
    public class GetQuestionByIdQueryValidator : AbstractValidator<GetQuestionByIdQuery>
    {
        public GetQuestionByIdQueryValidator()
        {
            RuleFor(x => x.QuestionId).NotEmpty().MaximumLength(200);
        }
    }
}
