using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StackOverflowService.WebRole
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var origins = (ConfigurationManager.AppSettings["Cors:AllowedOrigins"] ?? "").Replace(" ", "");
            var methods = ConfigurationManager.AppSettings["Cors:AllowedMethods"] ?? "GET,POST,PUT,PATCH,DELETE,OPTIONS";
            var headers = ConfigurationManager.AppSettings["Cors:AllowedHeaders"] ?? "Authorization,Content-Type,Accept,Origin,X-Requested-With";

            var cors = new EnableCorsAttribute(origins, headers, methods)
            {
                PreflightMaxAge = 600
            };
            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
