using FluentValidation;

namespace StackoverflowService.Application.Features.Users.SetUserPhoto
{
    public class SetUserPhotoCommandValidator : AbstractValidator<SetUserPhotoCommand>
    {
        public SetUserPhotoCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.File).NotNull();
            RuleFor(x => x.File.Content).NotNull().Must(c => c.Length > 0)
                .WithMessage("Image content is required.");
            RuleFor(x => x.File.ContentType).NotEmpty().Must(ct => ct.StartsWith("image/"))
                .WithMessage("Only image content types are allowed.");
            RuleFor(x => x.File.FileName).NotEmpty().MaximumLength(255);
        }
    }
}
