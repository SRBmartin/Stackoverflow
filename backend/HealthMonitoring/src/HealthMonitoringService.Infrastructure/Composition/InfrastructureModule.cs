using Autofac;
using HealthMonitoringService.Domain.Repositories;
using HealthMonitoringService.Infrastructure.Repositories;
using HealthMonitoringService.Infrastructure.Tables.Context;
using HealthMonitoringService.Infrastructure.Tables.Interfaces;

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

        }
    }
}
