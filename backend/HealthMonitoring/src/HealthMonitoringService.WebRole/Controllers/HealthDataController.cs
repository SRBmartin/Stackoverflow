using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using HealthMonitoringService.Application.Features.HealthCheck.GetLatest;

namespace HealthMonitoringService.WebRole.Controllers
{
    [RoutePrefix("api/healthdata")]
    public class HealthDataController : ApiController
    {
        private readonly IMediator _mediator;

        public HealthDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("latest")]
        public async Task<IHttpActionResult> GetLatest(CancellationToken cancellationToken)
        {
            var query = new GetLatestHealthChecksQuery();

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
