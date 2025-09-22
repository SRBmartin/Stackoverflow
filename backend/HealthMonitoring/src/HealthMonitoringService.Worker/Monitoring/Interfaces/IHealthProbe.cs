using System.Threading.Tasks;
using System.Threading;
using System;

namespace HealthMonitoringService.Worker.Monitoring.Interfaces
{
    public interface IHealthProbe
    {
        Task<bool> IsHealthyAsync(Uri url, CancellationToken cancellationToken);
    }
}
