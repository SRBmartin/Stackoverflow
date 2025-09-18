using MediatR;
using StackoverflowService.Application.Features.Votes;
using StackOverflowService.WebRole.Http;
using StackOverflowService.WebRole.Requests.Votes;
using StackOverflowService.WebRole.Security;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace StackOverflowService.WebRole.Controllers
{
    [RoutePrefix("api/votes")]
    public class VotesController : ApiController
    {
        private readonly IMediator _mediator;

        public VotesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Vote is available with types '+' and '-'.
        /// QuestionId is required and when it's only id sent, then vote goes to question.
        /// AnswerId is optional, and when it's sent then the vote goes to answer.
        /// </summary>
        /// <param name="request">RequestDTO, containing vote type (+ or -), question and/or answer id.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>204 No content on success</returns>
        [HttpPost, Route("")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> Vote(VoteOnAnswerRequest request, CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var command = new VoteCommand(
                userId: userId,
                questionId: request.QuestionId,
                answerId: request.AnswerId,
                type: request.Type
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess && result.Value)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return this.ToActionResult(result);
        }

    }
}
