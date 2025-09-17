using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using StackoverflowService.Application.Common;
using StackoverflowService.Application.Common.Results;

namespace StackOverflowService.WebRole.Http
{
    public static class ApiControllerExtensions
    {
        public static IHttpActionResult ToActionResult<T>(this ApiController controller, Result<T> res)
        {
            if (res == null)
                return new NegotiatedContentResult<object>(HttpStatusCode.BadRequest,
                    new { error = "Null result" }, controller);

            if (res.IsSuccess)
                return new NegotiatedContentResult<T>(HttpStatusCode.OK, res.Value, controller);

            var first = res.Errors?.FirstOrDefault()
                        ?? Error.Failure("Unknown", "Unknown error");
            var payload = new { code = first.Code, message = first.Message, errors = res.Errors ?? new Error[0] };

            return new NegotiatedContentResult<object>(MapStatus(first.Type), payload, controller);
        }

        public static IHttpActionResult ToActionResult(this ApiController controller, Result res)
        {
            if (res == null)
                return new NegotiatedContentResult<object>(HttpStatusCode.BadRequest,
                    new { error = "Null result" }, controller);

            if (res.IsSuccess)
                return new OkResult(controller);

            var first = res.Errors?.FirstOrDefault()
                        ?? Error.Failure("Unknown", "Unknown error");
            var payload = new { code = first.Code, message = first.Message, errors = res.Errors ?? new Error[0] };

            return new NegotiatedContentResult<object>(MapStatus(first.Type), payload, controller);
        }

        private static HttpStatusCode MapStatus(ErrorType t)
        {
            switch (t)
            {
                case ErrorType.NotFound: return HttpStatusCode.NotFound;
                case ErrorType.Conflict: return HttpStatusCode.Conflict;
                case ErrorType.Unauthorized: return HttpStatusCode.Unauthorized;
                case ErrorType.Forbidden: return HttpStatusCode.Forbidden;
                case ErrorType.Validation:
                case ErrorType.Failure:
                default: return HttpStatusCode.BadRequest;
            }
        }
    }
}
