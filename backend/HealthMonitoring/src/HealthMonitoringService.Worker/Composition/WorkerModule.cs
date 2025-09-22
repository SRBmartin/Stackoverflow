using Autofac;
using HealthMonitoringService.Worker.Monitoring.Interfaces;
using HealthMonitoringService.Worker.Monitoring;
using System.Net.Http;
using System;

namespace HealthMonitoringService.Worker.Composition
{
    public class WorkerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => MonitoringOptions.FromConfig())
                   .AsSelf()
                   .SingleInstance();

            builder.Register(ctx =>
            {
                var h = new HttpClientHandler();
                var http = new HttpClient(h, true)
                {
                    Timeout = TimeSpan.FromSeconds(3)
                };
                return http;
            })
            .Named<HttpClient>("ProbeClient")
            .SingleInstance();

            builder.Register(ctx =>
            {
                var http = ctx.ResolveNamed<HttpClient>("ProbeClient");
                return new HealthProbe(http);
            })
            .As<IHealthProbe>()
            .SingleInstance();

            builder.RegisterType<MonitoringService>()
                   .As<IMonitoringService>()
                   .SingleInstance();
        }
    }
}
