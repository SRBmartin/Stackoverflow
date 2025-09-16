using FluentValidation;

namespace StackoverflowService.Application.Features.Questions.GetQuestionPhoto
{
    public class GetQuestionPhotoQueryValidator : AbstractValidator<GetQuestionPhotoQuery>
    {
        public GetQuestionPhotoQueryValidator()
        {
            RuleFor(x => x.QuestionId).NotEmpty();
        }
    }
}
