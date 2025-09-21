using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;

namespace HealthMonitoringService.Infrastructure.Storage
{
    public static class StorageConnection
    {
        public static string Get()
        {
            try
            {
                if (RoleEnvironment.IsAvailable)
                    return RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString");
            }
            catch { /* ignored */ }

            var fromAppSettings = ConfigurationManager.AppSettings["StorageConnectionString"];
            if (!string.IsNullOrWhiteSpace(fromAppSettings)) return fromAppSettings;

            var cs = ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString;
            return cs ?? "UseDevelopmentStorage=true";
        }
    }
}
