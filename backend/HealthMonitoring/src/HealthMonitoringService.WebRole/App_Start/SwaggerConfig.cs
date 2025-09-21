using System.Web.Http;
using WebActivatorEx;
using HealthMonitoringService.WebRole;
using Swashbuckle.Application;
using System.Linq;
using System.IO;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace HealthMonitoringService.WebRole
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "HealthMonitoringService API");

                        c.RootUrl(request =>
                        {
                            //Get in runtime the actual running port since Azure service emulator is changing 
                            //sending wrong information

                            var scheme = request.Headers.Contains("X-Forwarded-Proto")
                                ? request.Headers.GetValues("X-Forwarded-Proto").FirstOrDefault()
                                : request.RequestUri.Scheme;

                            var host = request.Headers.Contains("X-Forwarded-Host")
                                ? request.Headers.GetValues("X-Forwarded-Host").FirstOrDefault()
                                : (request.Headers.Host ?? request.RequestUri.Authority);

                            return $"{scheme}://{host}";
                        });

                        // XML comments
                        var basePath = AppDomain.CurrentDomain.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "bin", "HealthMonitoringService.WebRole.xml");
                        if (File.Exists(xmlPath))
                            c.IncludeXmlComments(xmlPath);

                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("HealthMonitoringService API Docs");
                        c.DisableValidator();
                    });
        }
    }
}
