using Autofac;
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

        }
    }
}
