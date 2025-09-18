using FluentValidation;

namespace StackoverflowService.Application.Features.Users.GetUserPhoto
{
    public class GetUserPhotoQueryValidator : AbstractValidator<GetUserPhotoQuery>
    {
        public GetUserPhotoQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
