using HealthMonitoringService.Domain.Entities;
using HealthMonitoringService.Domain.Enums;
using HealthMonitoringService.Domain.Repositories;
using HealthMonitoringService.Infrastructure.Email;
using HealthMonitoringService.Worker.Monitoring.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService.Worker.Monitoring
{
    public class MonitoringService : IMonitoringService
    {
        private readonly MonitoringOptions _options;
        private readonly IHealthProbe _probe;
        private readonly IHealthCheckRepository _healthCheckRepository;
        private readonly IEmailService _emailService;

        public MonitoringService(
            MonitoringOptions options,
            IHealthProbe probe,
            IHealthCheckRepository checks,
            IEmailService emails)
        {
            _options = options;
            _probe = probe;
            _healthCheckRepository = checks;
            _emailService = emails;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var startedAt = DateTimeOffset.UtcNow;
                var nowUtc = startedAt;

                var tasks = _options.Targets
                    .Select(t => CheckTargetOnceAsync(t, nowUtc, cancellationToken))
                    .ToArray();

                await Task.WhenAll(tasks);

                var elapsed = DateTimeOffset.UtcNow - startedAt;
                var remaining = _options.Interval - elapsed;
                if (remaining > TimeSpan.Zero)
                {
                    try { await Task.Delay(remaining, cancellationToken); }
                    catch (OperationCanceledException) { /* shutting down */ }
                }
            }
        }

        #region Helpers

        private async Task CheckTargetOnceAsync(ServiceTarget t, DateTimeOffset nowUtc, CancellationToken ct)
        {
            try
            {
                bool ok = await _probe.IsHealthyAsync(t.Url, ct);

                var hc = new HealthCheck(nowUtc, ok ? HealthStatus.Healthy : HealthStatus.Unhealthy, t.Name);
                await _healthCheckRepository.AddAsync(hc, ct);

                if (!ok)
                {
                    await _emailService.SendServiceDownAsync(t.Name, nowUtc, ct);
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                //graceful shutdown
            }
            catch (Exception)
            {
                var hc = new HealthCheck(nowUtc, HealthStatus.Unhealthy, t.Name);
                await _healthCheckRepository.AddAsync(hc, ct);

                await _emailService.SendServiceDownAsync($"{t.Name}", nowUtc, ct);
            }
        }

        #endregion

    }
}
