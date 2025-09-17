using FluentValidation;

namespace StackoverflowService.Application.Features.Questions.UpdateQuestion
{
    public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
    {
        public UpdateQuestionCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.QuestionId).NotEmpty();

            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters long.");

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.")
                .MaximumLength(10000).WithMessage("Description must be at most 10000 characters long.");
        }
    }
}
