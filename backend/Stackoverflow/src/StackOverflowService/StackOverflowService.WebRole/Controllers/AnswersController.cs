using MediatR;
using StackoverflowService.Application.Features.Answers.CreateAnswer;
using StackoverflowService.Application.Features.Answers.SetAnswerAsFinal;
using StackOverflowService.WebRole.Http;
using StackOverflowService.WebRole.Requests.Answers;
using StackOverflowService.WebRole.Security;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace StackOverflowService.WebRole.Controllers
{
    [RoutePrefix("api/answers")]
    public class AnswersController : ApiController
    {
        private readonly IMediator _mediator;

        public AnswersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> Create(CreateAnswerRequest request, CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Content(HttpStatusCode.Unauthorized, "Unauthorized");

            var command = new CreateAnswerCommand(
                userId: userId,
                questionId: request.QuestionId,
                text: request.Text
            );

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpPost, Route("final")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> SetFinal(SetAnswerAsFinalRequest request, CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Content(HttpStatusCode.Unauthorized, "Unauthorized");
            
            var command = new SetAnswerAsFinalCommand(userId, request.QuestionId, request.AnswerId);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess && result.Value)
                return StatusCode(HttpStatusCode.NoContent);

            return this.ToActionResult(result);
        }

    }
}
