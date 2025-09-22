using System;

namespace HealthMonitoringService.Worker.Monitoring
{
    public class ServiceTarget
    {
        public string Name { get; private set; }
        public Uri Url { get; private set; }

        public ServiceTarget(string name, Uri url)
        {
            Name = (name ?? string.Empty).Trim();
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

    }
}
