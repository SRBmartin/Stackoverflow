using FluentValidation;

namespace StackoverflowService.Application.Features.Answers.SetAnswerAsFinal
{
    public class SetAnswerAsFinalCommandValidator : AbstractValidator<SetAnswerAsFinalCommand>
    {
        public SetAnswerAsFinalCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.QuestionId).NotEmpty().MaximumLength(200);
            RuleFor(x => x.AnswerId).NotEmpty().MaximumLength(200);
        }
    }
}
