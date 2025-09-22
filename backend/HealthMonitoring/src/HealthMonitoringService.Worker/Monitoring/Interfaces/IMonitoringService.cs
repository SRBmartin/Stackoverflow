using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService.Worker.Monitoring.Interfaces
{
    public interface IMonitoringService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
