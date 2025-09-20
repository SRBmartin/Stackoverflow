using System.Web.Http;
using WebActivatorEx;
using StackOverflowService.WebRole;
using Swashbuckle.Application;
using System.IO;
using System;
using StackOverflowService.WebRole.Swagger;
using System.Linq;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace StackOverflowService.WebRole
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "StackOverflowService API");

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
                        var xmlPath = Path.Combine(basePath, "bin", "StackOverflowService.WebRole.xml");
                        if (File.Exists(xmlPath))
                            c.IncludeXmlComments(xmlPath);

                        c.OperationFilter<FileUploadOperationFilter>();
                        c.OperationFilter<FileResponseOperationFilter>();

                        c.ApiKey("Bearer")
                            .Description("JWT Authorization header using the Bearer scheme. Example: \"Bearer <token>\".")
                            .Name("Authorization")
                            .In("header");
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("StackOverflowService API Docs");
                        c.DisableValidator();
                        c.EnableApiKeySupport("Authorization", "header");
                    });
        }
    }
}
