using Autofac;
using HealthMonitoringService.Domain.Repositories;
using HealthMonitoringService.Infrastructure.Configuration;
using HealthMonitoringService.Infrastructure.Repositories;
using HealthMonitoringService.Infrastructure.Tables.Context;
using HealthMonitoringService.Infrastructure.Tables.Interfaces;
using System.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using HealthMonitoringService.Infrastructure.Email;

namespace HealthMonitoringService.Infrastructure.Composition
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TableContext>()
                .As<ITableContext>()
                .SingleInstance()
                .AutoActivate();

            builder.RegisterType<HealthCheckRepository>()
                .As<IHealthCheckRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AlertEmailRepository>()
                .As<IAlertEmailRepository>()
                .InstancePerLifetimeScope();

            builder.Register(ctx =>
            {
                var baseUrl = ConfigurationManager.AppSettings["EmailApi:BaseAddress"];
                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new ConfigurationErrorsException("Missing appSettings key 'EmailApi:BaseAddress'.");

                int timeout = 100;
                var raw = ConfigurationManager.AppSettings["EmailApi:TimeoutSeconds"];
                if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw, out var t) && t > 0) timeout = t;

                var token = ConfigurationManager.AppSettings["EmailApi:Token"] ?? string.Empty;

                return new EmailApiOptions
                {
                    BaseAddress = new Uri(baseUrl, UriKind.Absolute),
                    TimeoutSeconds = timeout,
                    Token = token
                };
            })
            .AsSelf()
            .SingleInstance();

            builder.Register(ctx =>
            {
                var opts = ctx.Resolve<EmailApiOptions>();
                var http = new HttpClient(new HttpClientHandler(), disposeHandler: true)
                {
                    BaseAddress = opts.BaseAddress,
                    Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds)
                };
                http.DefaultRequestHeaders.Accept.Clear();
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(opts.Token))
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(opts.Token);
                }

                return http;
            })
            .Named<HttpClient>("EmailApiClient")
            .SingleInstance();

            builder.Register(ctx =>
            {
                var http = ctx.ResolveNamed<HttpClient>("EmailApiClient");
                var alerts = ctx.Resolve<IAlertEmailRepository>();
                var checks = ctx.Resolve<IHealthCheckRepository>();
                return new EmailService(http, alerts);
            })
            .As<IEmailService>()
            .SingleInstance();

        }
    }
}
