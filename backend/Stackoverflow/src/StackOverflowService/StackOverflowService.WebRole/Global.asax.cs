using Autofac;
using Autofac.Integration.WebApi;
using StackoverflowService.Application.Composition;
using StackoverflowService.Infrastructure.Composition;
using System;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace StackOverflowService.WebRole
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterModule(new InfrastructureModule());
            builder.RegisterModule(new ApplicationModule());

            var container = builder.Build();

            var resolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var ctx = HttpContext.Current;
            var path = ctx?.Request?.Url?.AbsolutePath ?? "";
            if (string.Equals(path, "/", StringComparison.Ordinal))
            {
                ctx.Response.Redirect("~/swagger/ui/index", endResponse: true);
            }

        }

    }
}
