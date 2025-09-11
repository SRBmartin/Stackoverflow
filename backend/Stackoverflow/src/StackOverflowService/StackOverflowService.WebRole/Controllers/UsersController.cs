using MediatR;
using StackoverflowService.Application.Features.Users.RegisterUser;
using StackOverflowService.WebRole.Http;
using StackOverflowService.WebRole.Requests.User;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace StackOverflowService.WebRole.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Register(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(request.Name, request.Lastname, request.Email, request.Password, request.Gender, request.State, request.City, request.Address);

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

    }
}
