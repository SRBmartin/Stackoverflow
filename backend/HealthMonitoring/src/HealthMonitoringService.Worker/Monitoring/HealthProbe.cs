using HealthMonitoringService.Worker.Monitoring.Interfaces;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService.Worker.Monitoring
{
    public class HealthProbe : IHealthProbe
    {
        private readonly HttpClient _httpClient;
        public HealthProbe(HttpClient http)
        {
            _httpClient = http;
        }
        public async Task<bool> IsHealthyAsync(Uri url, CancellationToken cancellationToken)
        {
            try
            {
                using (var req = new HttpRequestMessage(HttpMethod.Get, url))
                using (var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    var ok = (int)resp.StatusCode == 200;
                    if (!ok)
                        Trace.TraceWarning($"[HealthProbe]: {url} -> {(int)resp.StatusCode} {resp.ReasonPhrase}");
                    return ok;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[HealthProbe]: exception for {url}: {ex.Message}");

                return false;
            }
        }
    }
}
