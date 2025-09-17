using FluentValidation;

namespace StackoverflowService.Application.Features.Votes
{
    public class VoteCommandValidator : AbstractValidator<VoteCommand>
    {
        public VoteCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();

            RuleFor(x => x.QuestionId)
                .NotEmpty()
                .MaximumLength(200);

            When(x => !string.IsNullOrWhiteSpace(x.AnswerId), () =>
            {
                RuleFor(x => x.AnswerId)
                    .NotEmpty()
                    .MaximumLength(200);
            });

            RuleFor(x => x.Type)
                .NotEmpty()
                .Must(IsSupportedType)
                .WithMessage("Invalid vote 'type'. Use '+', '-'.");

        }

        private static bool IsSupportedType(string t)
        {
            if (string.IsNullOrWhiteSpace(t)) return false;
            var v = t.Trim().ToLowerInvariant();
            return v == "+" || v == "-";
        }

    }
}
