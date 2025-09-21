using Azure.Data.Tables;
using HealthMonitoringService.Infrastructure.Storage;
using HealthMonitoringService.Infrastructure.Tables.Interfaces;

namespace HealthMonitoringService.Infrastructure.Tables.Context
{
    public class TableContext : ITableContext
    {
        public readonly TableServiceClient _svc;

        public TableClient AlertEmails { get; }
        public TableClient HealthChecks { get; }

        public TableContext()
        {
            _svc = new TableServiceClient(StorageConnection.Get());

            AlertEmails = _svc.GetTableClient(TableNames.AlertEmails);
            HealthChecks = _svc.GetTableClient(TableNames.HealthChecks);

            AlertEmails.CreateIfNotExists();
            HealthChecks.CreateIfNotExists();
        }

    }
}
