using Azure.Data.Tables;
using HealthMonitoringService.Domain.Entities;
using HealthMonitoringService.Domain.Repositories;
using HealthMonitoringService.Infrastructure.Mapping;
using HealthMonitoringService.Infrastructure.Tables.Entities;
using HealthMonitoringService.Infrastructure.Tables.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace HealthMonitoringService.Infrastructure.Repositories
{
    public class HealthCheckRepository : IHealthCheckRepository
    {
        private readonly TableClient _healthChecks;

        public HealthCheckRepository(ITableContext tableContext)
        {
            _healthChecks = tableContext.HealthChecks;
        }

        public async Task AddAsync(HealthCheck check, CancellationToken cancellationToken)
        {
            var entity = check.ToTable();

            await _healthChecks.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
        }

        public async Task<HealthCheck?> GetLatestAsync(string serviceName, CancellationToken cancellationToken)
        {
            var pk = HealthCheckEntity.PartitionFrom(serviceName);

            var filter = TableClient.CreateQueryFilter<HealthCheckEntity>(e => e.PartitionKey == pk);

            await foreach (var e in _healthChecks.QueryAsync<HealthCheckEntity>(
                filter: filter, maxPerPage: 1, cancellationToken: cancellationToken))
            {
                //RowKey is inverted ticks, so the first item is the newest
                return e.ToDomain();
            }

            return null;
        }

        public async Task<IReadOnlyList<HealthCheck>> GetLatestAsync(string serviceName, int take, CancellationToken cancellationToken)
        {
            var pk = HealthCheckEntity.PartitionFrom(serviceName);
            var filter = TableClient.CreateQueryFilter<HealthCheckEntity>(e => e.PartitionKey == pk);

            var list = new List<HealthCheck>(take > 0 ? take : 4);

            await foreach (var e in _healthChecks.QueryAsync<HealthCheckEntity>(
                filter: filter, maxPerPage: take > 0 ? take : default(int?), cancellationToken: cancellationToken))
            {
                list.Add(e.ToDomain());
                if (take > 0 && list.Count >= take) break;
            }

            return list;
        }
    }
}
