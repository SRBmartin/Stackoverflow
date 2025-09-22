using HealthMonitoringService.Domain.Entities;
using HealthMonitoringService.Domain.Enums;
using HealthMonitoringService.Infrastructure.Tables.Entities;
using System;

namespace HealthMonitoringService.Infrastructure.Mapping
{
    public static class TableMappings
    {
        public static AlertEmailEntity ToTable(this AlertEmail a)
        {
            var keys = AlertEmailEntity.KeysFromEmail(a.Email);
            return new AlertEmailEntity
            {
                PartitionKey = keys.pk,
                RowKey = keys.rk,
                Email = a.Email
            };
        }

        public static AlertEmail ToDomain(this AlertEmailEntity e)
            => new AlertEmail(e.Email);

        public static HealthCheckEntity ToTable(this HealthCheck c)
            => new HealthCheckEntity
            {
                PartitionKey = HealthCheckEntity.PartitionFrom(c.ServiceName),
                RowKey = HealthCheckEntity.RowKeyFrom(c.DateTime),
                DateTime = c.DateTime,
                Status = HealthCheckEntity.StatusFrom(c.Status),
                ServiceName = c.ServiceName
            };

        public static HealthCheck ToDomain(this HealthCheckEntity e)
        {
            HealthStatus status;
            if (!Enum.TryParse(e.Status, ignoreCase: true, out status))
                status = HealthStatus.Unhealthy;

            return new HealthCheck(
                e.DateTime,
                status,
                e.ServiceName
            );
        }
    }
}
