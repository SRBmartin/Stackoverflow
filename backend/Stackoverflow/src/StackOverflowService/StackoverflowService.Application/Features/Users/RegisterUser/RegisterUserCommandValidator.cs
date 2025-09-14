using FluentValidation;
using System.Linq;

namespace StackoverflowService.Application.Features.Users.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Lastname).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(200);
            RuleFor(x => x.Gender)
                .Must(g => new[] { "M", "F" }.Contains((g ?? "").ToUpperInvariant()))
                .WithMessage("Gender must be one of 'M', 'F'.");
            RuleFor(x => x.State).MaximumLength(100);
            RuleFor(x => x.City).MaximumLength(100);
            RuleFor(x => x.Address).MaximumLength(200);
        }
    }
}
