using System;
using System.Configuration;

namespace HealthMonitoringService.Worker.Monitoring
{
    public class MonitoringOptions
    {
        public ServiceTarget[] Targets { get; private set; }
        public TimeSpan Interval { get; private set; }

        public static MonitoringOptions FromConfig()
        {
            var soFallback = GetRequired("HealthEndpoints:Stackoverflow");
            var notifFallback = GetRequired("HealthEndpoints:NotificationService");

            var soUrl = EndpointResolver.ResolveOrFallback("StackOverflowService.WebRole", "Endpoint1", soFallback);
            var notifUrl = EndpointResolver.ResolveOrFallback("NotificationService", "Health", notifFallback);

            return new MonitoringOptions
            {
                Targets = new[]
                {
                    new ServiceTarget("Stackoverflow",        soUrl),
                    new ServiceTarget("NotificationService",  notifUrl)
                },
                Interval = TimeSpan.FromSeconds(4)
            };
        }

        private static string GetRequired(string key)
        {
            var v = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(v))
                throw new ConfigurationErrorsException("Missing appSettings key '" + key + "'.");
            return v;
        }
    }
}
