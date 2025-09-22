using HealthMonitoringService.Domain.Enums;
using System;

namespace HealthMonitoringService.Domain.Entities
{
    public class HealthCheck
    {
        public DateTimeOffset DateTime { get; }
        public HealthStatus Status { get; }
        public string ServiceName { get; }

        public HealthCheck(
            DateTimeOffset dateTime,
            HealthStatus status,
            string serviceName
        )
        {
            DateTime = dateTime;
            Status = status;
            ServiceName = serviceName;
        }

    }
}
