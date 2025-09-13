using System.Net.Http.Headers;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Net.Http;
using System;
using StackoverflowService.Application.Abstractions;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace StackOverflowService.WebRole.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireJwtAuth : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext)) return;

            var request = actionContext.Request;
            var auth = request.Headers.Authorization;

            if (auth == null || !auth.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(auth.Parameter))
            {
                Deny(actionContext, "invalid_request", "Missing or invalid Authorization header.");
                return;
            }

            var validator = (IIdentityValidator)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IIdentityValidator));

            if (validator == null)
            {
                Deny(actionContext, "server_error", "JWT validator not available.");
                return;
            }

            if (!validator.TryValidate(auth.Parameter, out ClaimsPrincipal principal, out var error))
            {
                Deny(actionContext, "invalid_token", error ?? "Invalid token.");
                return;
            }

            actionContext.RequestContext.Principal = principal;
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null) HttpContext.Current.User = principal;
        }

        private static bool SkipAuthorization(HttpActionContext ctx)
        {
            //For potential [AllowAnonymous]
            return ctx.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0
                || ctx.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0;
        }

        private static void Deny(HttpActionContext ctx, string error, string description)
        {
            var response = ctx.Request.CreateResponse(HttpStatusCode.Unauthorized, new
            {
                error,
                error_description = description
            });
            response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Bearer", $"error=\"{error}\""));
            ctx.Response = response;
        }

    }
}