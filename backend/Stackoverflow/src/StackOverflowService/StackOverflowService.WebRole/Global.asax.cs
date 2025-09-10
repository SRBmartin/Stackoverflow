using Autofac;
using Autofac.Integration.WebApi;
using StackoverflowService.Infrastructure.Composition;
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

            var container = builder.Build();

            var resolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}
