using Microsoft.WindowsAzure.ServiceRuntime;
using System;

namespace HealthMonitoringService.Worker.Monitoring
{
    internal static class EndpointResolver
    {
        public static Uri ResolveOrFallback(string roleName, string endpointName, string fallbackUrl)
        {
            try
            {
                if (RoleEnvironment.IsAvailable &&
                    RoleEnvironment.Roles.ContainsKey(roleName) &&
                    RoleEnvironment.Roles[roleName].Instances.Count > 0)
                {
                    var ep = RoleEnvironment.Roles[roleName]
                              .Instances[0]
                              .InstanceEndpoints[endpointName]
                              .IPEndpoint;

                    var host = ep.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                        ? $"[{ep.Address}]"
                        : ep.Address.ToString();

                    return new Uri($"http://{host}:{ep.Port}/health-monitoring", UriKind.Absolute);
                }
            }
            catch { /* fall through to fallback */ }

            return new Uri(fallbackUrl, UriKind.Absolute);
        }
    }
}
