using Azure;
using Azure.Data.Tables;
using HealthMonitoringService.Domain.Enums;
using System;

namespace HealthMonitoringService.Infrastructure.Tables.Entities
{
    public class HealthCheckEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default!; // service name (lowercase)
        public string RowKey { get; set; } = default!; //inverted ticks

        public DateTimeOffset DateTime { get; set; }
        public string Status { get; set; } = default!; //"Healthy" or "Unhealthy"
        public string ServiceName { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public static string PartitionFrom(string serviceName) =>
            (serviceName ?? "unknown").Trim().ToLowerInvariant();

        public static string RowKeyFrom(DateTimeOffset dtUtc)
        {
            long inverted = DateTimeOffset.MaxValue.Ticks - dtUtc.UtcTicks;
            return inverted.ToString("D19");
        }

        public static string StatusFrom(HealthStatus status) => status.ToString();
    }
}
