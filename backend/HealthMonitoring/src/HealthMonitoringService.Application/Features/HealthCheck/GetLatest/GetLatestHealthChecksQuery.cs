using HealthMonitoringService.Application.DTOs.HealthCheck;
using MediatR;
using System.Collections.Generic;

namespace HealthMonitoringService.Application.Features.HealthCheck.GetLatest
{
    public class GetLatestHealthChecksQuery : IRequest<IReadOnlyList<HealthCheckDto>> { }
}
