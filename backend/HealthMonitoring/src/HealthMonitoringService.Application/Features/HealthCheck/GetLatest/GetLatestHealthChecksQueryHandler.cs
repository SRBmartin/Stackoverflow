using HealthMonitoringService.Application.DTOs.HealthCheck;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using HealthMonitoringService.Domain.Repositories;
using System.Linq;

namespace HealthMonitoringService.Application.Features.HealthCheck.GetLatest
{
    class GetLatestHealthChecksQueryHandler : IRequestHandler<GetLatestHealthChecksQuery, IReadOnlyList<HealthCheckDto>>
    {
        private readonly IHealthCheckRepository _repo;

        public GetLatestHealthChecksQueryHandler(IHealthCheckRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<HealthCheckDto>> Handle(GetLatestHealthChecksQuery request, CancellationToken cancellationToken)
        {
            var nowUtc = DateTimeOffset.UtcNow;
            var sinceUtc = nowUtc.AddHours(-1); //-3 UTC+2

            var checks = await _repo.GetSinceAsync(sinceUtc, cancellationToken);

            return checks.Select(h => new HealthCheckDto
            {
                DateTimeUtc = h.DateTime,
                Status = h.Status.ToString(),
                ServiceName = h.ServiceName
            }).ToList();
        }
    }
}
