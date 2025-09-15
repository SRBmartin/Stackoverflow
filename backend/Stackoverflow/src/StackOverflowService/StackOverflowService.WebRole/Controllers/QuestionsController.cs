using MediatR;

using System.Web.Http;
using StackOverflowService.WebRole.Security;
using System.Threading.Tasks;
using StackOverflowService.WebRole.Requests.Questions;
using System.Threading;
using System.Net;
using System.Security.Claims;
using StackoverflowService.Application.Features.Questions.CreateQuestion;
using StackOverflowService.WebRole.Http;

namespace StackOverflowService.WebRole.Controllers
{
    [RoutePrefix("api/questions")]
    public class QuestionsController : ApiController
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> Create(CreateQuestionRequest request, CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Content(HttpStatusCode.Unauthorized, "Unauthorized");
            }

            var command = new CreateQuestionCommand(userId, request.Title, request.Description);

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

    }
}
