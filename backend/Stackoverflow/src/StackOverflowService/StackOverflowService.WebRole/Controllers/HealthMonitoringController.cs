using System.Threading.Tasks;
using System.Web.Http;

namespace StackOverflowService.WebRole.Controllers
{
    [RoutePrefix("")]
    public class HealthMonitoringController : ApiController
    {
        [HttpGet, Route("health-monitoring")]
        [AllowAnonymous]
        public IHttpActionResult HealthStatus()
        {
            return Ok();
        }

    }
}
