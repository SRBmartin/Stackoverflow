using HealthMonitoringService.Application.DTOs.Email;
using HealthMonitoringService.Domain.Repositories;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _http;
        private readonly IAlertEmailRepository _alertRepository;

        public EmailService(
            HttpClient http,
            IAlertEmailRepository alertRepository)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
        }

        public async Task<int> SendServiceDownAsync(string serviceName, DateTimeOffset detectedAtUtc, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Service name is required.", nameof(serviceName));

            var recipients = await _alertRepository.GetAllAsync(ct);
            if (recipients == null || recipients.Count == 0) return 0;

            var html = BuildEmailBody(serviceName, detectedAtUtc);

            var sent = 0;
            foreach (var r in recipients)
            {
                if (string.IsNullOrWhiteSpace(r.Email)) continue;

                var mail = new OutgoingMail
                {
                    To = r.Email,
                    Subject = $"[Health-SO] Service DOWN: {serviceName}",
                    Body = html
                };

                var json = JsonConvert.SerializeObject(mail);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var resp = await _http.PostAsync("Mail/send", content, ct))
                {
                    if (resp.IsSuccessStatusCode) sent++;
                }
            }

            return sent;
        }

        #region Helpers

        private static string BuildEmailBody(string serviceName, DateTimeOffset detectedAtUtc)
        {
            var template = LoadTemplate();

            var (localTime, tzDisplay) = ConvertToBelgrade(detectedAtUtc);

            var safeService = WebUtility.HtmlEncode(serviceName ?? string.Empty);
            var utcText = detectedAtUtc.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss 'UTC'");
            var localText = localTime.ToString($"yyyy-MM-dd HH:mm:ss '{tzDisplay}'");

            return template
                .Replace("{{ServiceName}}", safeService)
                .Replace("{{DetectedAtUtc}}", utcText)
                .Replace("{{DetectedAtLocal}}", localText);
        }

        private static string LoadTemplate()
        {
            var asm = typeof(EmailService).Assembly;
            var name = asm.GetManifestResourceNames()
                          .FirstOrDefault(n => n.EndsWith("Email.Templates.ServiceDown.html", StringComparison.OrdinalIgnoreCase));

            if (name == null)
                throw new InvalidOperationException("ServiceDown.html not found as an embedded resource.");

            using (var stream = asm.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private static (DateTimeOffset local, string display) ConvertToBelgrade(DateTimeOffset utc)
        {
            TimeZoneInfo tz = null;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            }
            catch
            {
                try { tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Belgrade"); } catch { }
            }

            if (tz == null)
            {
                var fixedOffset = TimeSpan.FromHours(2);
                return (new DateTimeOffset(utc.UtcDateTime, fixedOffset), "UTC+02:00");
            }

            var local = TimeZoneInfo.ConvertTime(utc, tz);
            var offset = local.Offset;
            var label = $"UTC{(offset.TotalMinutes >= 0 ? "+" : "-")}{offset:hh\\:mm}";
            return (local, label);
        }

        #endregion

    }
}
