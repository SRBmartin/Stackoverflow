using HealthMonitoringService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

#nullable enable

namespace HealthMonitoringService.Domain.Repositories
{
    public interface IHealthCheckRepository
    {
        Task AddAsync(HealthCheck check, CancellationToken cancellationToken);
        Task<HealthCheck?> GetLatestAsync(string serviceName, CancellationToken cancellationToken); //returns null if none
        Task<IReadOnlyList<HealthCheck>> GetLatestAsync(string serviceName, int take, CancellationToken cancellationToken);
    }
}
