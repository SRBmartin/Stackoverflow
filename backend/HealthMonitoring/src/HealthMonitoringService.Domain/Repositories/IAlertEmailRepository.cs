using HealthMonitoringService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace HealthMonitoringService.Domain.Repositories
{
    public interface IAlertEmailRepository
    {
        Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);
        Task AddAsync(AlertEmail email, CancellationToken cancellationToken);
        Task RemoveAsync(string email, CancellationToken cancellationToken);
        Task<IReadOnlyList<AlertEmail>> GetAllAsync(CancellationToken cancellationToken);
    }
}
