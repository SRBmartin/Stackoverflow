using FluentValidation;

namespace StackoverflowService.Application.Features.Answers.CreateAnswer
{
    public class CreateAnswerCommandValidator : AbstractValidator<CreateAnswerCommand>
    {
        public CreateAnswerCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.QuestionId).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Text).NotEmpty().MinimumLength(1).MaximumLength(10_000);
        }
    }
}
