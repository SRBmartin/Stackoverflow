using MediatR;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Users;
using StackoverflowService.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Features.Users.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserProfileQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(query.UserId, cancellationToken);

            if (user == null)
            {
                return Result.Fail<UserDto>(Error.NotFound("User.NotFound", "User not found."));
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email,
                Gender = user.Gender == Domain.Enums.Gender.Male ? "M" : "F",
                State = user.State,
                City = user.City,
                Address = user.Address,
                PhotoBlobName = user.Photo?.BlobName,
                PhotoContainer = user.Photo?.Container,
                CreationDate = user.CreationDate
            };

            return Result.Ok(userDto);
        }
    }
}
