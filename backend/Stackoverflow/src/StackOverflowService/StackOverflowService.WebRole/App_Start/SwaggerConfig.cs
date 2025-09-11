using System.Web.Http;
using WebActivatorEx;
using StackOverflowService.WebRole;
using Swashbuckle.Application;
using System.IO;
using System;
using StackOverflowService.WebRole.Swagger;

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

                        // XML comments (optional but recommended)
                        var basePath = AppDomain.CurrentDomain.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "bin", "StackOverflowService.WebRole.xml");
                        if (File.Exists(xmlPath))
                            c.IncludeXmlComments(xmlPath);

                        c.OperationFilter<FileUploadOperationFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("StackOverflowService API Docs");
                        c.DisableValidator();
                    });
        }
    }
}
