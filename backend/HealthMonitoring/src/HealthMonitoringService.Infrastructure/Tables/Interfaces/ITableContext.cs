using Azure.Data.Tables;

namespace HealthMonitoringService.Infrastructure.Tables.Interfaces
{
    public interface ITableContext
    {
        TableClient AlertEmails { get; }
        TableClient HealthChecks { get; }
    }
}
