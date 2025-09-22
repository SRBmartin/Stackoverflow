using Azure;
using Azure.Data.Tables;
using System;


namespace HealthMonitoringService.Infrastructure.Tables.Entities
{
    public class AlertEmailEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;

        public string Email { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public static (string pk, string rk) KeysFromEmail(string email)
        {
            var e = (email ?? string.Empty).Trim().ToLowerInvariant();
            var at = e.IndexOf('@');

            string domain;
            if (at > 0 && at < e.Length - 1)
            {
                domain = e.Substring(at + 1);
            }
            else
            {
                domain = "default";
            }

            return (domain, e);
        }
    }
}
