using System.Threading.Tasks;
using System.Threading;
using System;

namespace HealthMonitoringService.Infrastructure.Email
{
    public interface IEmailService
    {
        Task<int> SendServiceDownAsync(string serviceName, DateTimeOffset detectedAtUtc, CancellationToken ct);
    }
}
