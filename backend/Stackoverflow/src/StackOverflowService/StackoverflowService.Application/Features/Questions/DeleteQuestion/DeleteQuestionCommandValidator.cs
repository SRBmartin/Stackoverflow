using FluentValidation;

namespace StackoverflowService.Application.Features.Questions.DeleteQuestion
{
    public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
    {
        public DeleteQuestionCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.QuestionId).NotEmpty();
        }
    }
}
