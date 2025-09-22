using System;

namespace HealthMonitoringService.Application.DTOs.HealthCheck
{
    public class HealthCheckDto
    {
        public DateTimeOffset DateTimeUtc { get; set; }
        public string Status { get; set; }
        public string ServiceName { get; set; } = default;
    }
}
