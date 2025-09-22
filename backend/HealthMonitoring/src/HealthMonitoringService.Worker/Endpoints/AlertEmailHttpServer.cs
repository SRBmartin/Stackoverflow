using Autofac;
using HealthMonitoringService.Domain.Repositories;
using HealthMonitoringService.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HealthMonitoringService.Worker.Endpoints
{
    public sealed class AlertEmailHttpServer : IDisposable
    {
        private readonly IContainer _container;
        private HttpListener _listener;
        private Task _loopTask;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public AlertEmailHttpServer(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void Start(IPEndPoint endpoint)
        {
            if (_listener != null) return;

            var host = endpoint.Address.AddressFamily == AddressFamily.InterNetworkV6
                ? $"[{endpoint.Address}]"
                : endpoint.Address.ToString();

            var prefix = $"http://{host}:{endpoint.Port}/";

            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _listener.Start();

            Trace.TraceInformation($"[AlertEmailApi] Listening at {prefix}");
            _loopTask = Task.Run(() => LoopAsync(_cts.Token));
        }

        private async Task LoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                HttpListenerContext ctx = null;
                try
                {
                    ctx = await _listener.GetContextAsync();
                    await HandleAsync(ctx, ct);
                }
                catch (ObjectDisposedException) { /* shutting down */ }
                catch (HttpListenerException) when (ct.IsCancellationRequested) { /* shutting down */ }
                catch (Exception ex)
                {
                    Trace.TraceError("[AlertEmailApi] Unhandled error: " + ex);
                    TryWriteStatus(ctx?.Response, 500, "application/json", JsonConvert.SerializeObject(new { error = "internal_error" }));
                }
            }
        }

        private async Task HandleAsync(HttpListenerContext ctx, CancellationToken ct)
        {
            var path = (ctx.Request.Url?.AbsolutePath ?? "/").TrimEnd('/').ToLowerInvariant();
            var method = ctx.Request.HttpMethod?.ToUpperInvariant();

            if (path == "/alert/email/list") path = "/alertemail/list";

            switch (path)
            {
                case "/alertemail/add" when method == "POST": await AddAsync(ctx, ct); return;
                case "/alertemail/update" when method == "POST": await UpdateAsync(ctx, ct); return;
                case "/alertemail/list" when method == "GET": await ListAsync(ctx, ct); return;
                case "/alertemail/delete" when method == "POST": await DeleteAsync(ctx, ct); return;
                default:
                    TryWriteStatus(ctx.Response, 404, "text/plain", "Not Found");
                    return;
            }
        }

        private async Task AddAsync(HttpListenerContext ctx, CancellationToken ct)
        {
            var body = await ReadBodyAsync(ctx.Request);
            var req = TryDeserialize<AddAlertEmailRequest>(body);

            if (req == null || string.IsNullOrWhiteSpace(req.Email))
            {
                TryWriteStatus(ctx.Response, 400, "application/json", JsonConvert.SerializeObject(new { error = "invalid_request" }));
                return;
            }

            using (var scope = _container.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IAlertEmailRepository>();
                await repo.AddAsync(new AlertEmail(req.Email), ct);
            }

            TryWriteStatus(ctx.Response, 200, "application/json", JsonConvert.SerializeObject(new { ok = true }));
        }

        private async Task UpdateAsync(HttpListenerContext ctx, CancellationToken ct)
        {
            var body = await ReadBodyAsync(ctx.Request);
            var req = TryDeserialize<UpdateAlertEmailRequest>(body);

            if (req == null || string.IsNullOrWhiteSpace(req.OldEmail) || string.IsNullOrWhiteSpace(req.NewEmail))
            {
                TryWriteStatus(ctx.Response, 400, "application/json", JsonConvert.SerializeObject(new { error = "invalid_request" }));
                return;
            }

            using (var scope = _container.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IAlertEmailRepository>();

                var exists = await repo.ExistsAsync(req.OldEmail, ct);
                if (!exists)
                {
                    TryWriteStatus(ctx.Response, 404, "application/json", JsonConvert.SerializeObject(new { error = "not_found" }));
                    return;
                }

                // "Update" = delete old, add new
                await repo.RemoveAsync(req.OldEmail, ct);
                await repo.AddAsync(new AlertEmail(req.NewEmail), ct);
            }

            TryWriteStatus(ctx.Response, 200, "application/json", JsonConvert.SerializeObject(new { ok = true }));
        }

        private async Task ListAsync(HttpListenerContext ctx, CancellationToken ct)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IAlertEmailRepository>();
                var items = await repo.GetAllAsync(ct);

                var resp = new ListAlertEmailResponse
                {
                    Items = items is null ? Array.Empty<string>() : Array.ConvertAll(items.ToArray(), x => x.Email)
                };

                TryWriteStatus(ctx.Response, 200, "application/json", JsonConvert.SerializeObject(resp));
            }
        }

        private async Task DeleteAsync(HttpListenerContext ctx, CancellationToken ct)
        {
            string email = ctx.Request.QueryString["email"];
            if (string.IsNullOrWhiteSpace(email))
            {
                var body = await ReadBodyAsync(ctx.Request);
                var req = TryDeserialize<DeleteAlertEmailRequest>(body);
                email = req?.Email;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                TryWriteStatus(ctx.Response, 400, "application/json", JsonConvert.SerializeObject(new { error = "invalid_request" }));
                return;
            }

            using (var scope = _container.BeginLifetimeScope())
            {
                var repo = scope.Resolve<IAlertEmailRepository>();
                await repo.RemoveAsync(email, ct);
            }

            TryWriteStatus(ctx.Response, 200, "application/json", JsonConvert.SerializeObject(new { ok = true }));
        }

        private static async Task<string> ReadBodyAsync(HttpListenerRequest req)
        {
            using (var sr = new StreamReader(req.InputStream, req.ContentEncoding ?? Encoding.UTF8))
                return await sr.ReadToEndAsync();
        }

        private static T TryDeserialize<T>(string json) where T : class
        {
            try { return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<T>(json); }
            catch { return null; }
        }

        private static void TryWriteStatus(HttpListenerResponse resp, int code, string contentType, string payload)
        {
            if (resp == null) return;
            try
            {
                resp.StatusCode = code;
                resp.ContentType = contentType;
                var bytes = Encoding.UTF8.GetBytes(payload ?? "");
                resp.ContentLength64 = bytes.LongLength;
                using (var s = resp.OutputStream) { s.Write(bytes, 0, bytes.Length); }
            }
            catch {  }
        }

        public void Dispose()
        {
            try { _cts.Cancel(); } catch { }
            try { _listener?.Stop(); } catch { }
            try { _listener?.Close(); } catch { }
            try { _loopTask?.Wait(TimeSpan.FromSeconds(2)); } catch { }
        }

        #region DTO
        private sealed class AddAlertEmailRequest { public string Email { get; set; } }
        private sealed class UpdateAlertEmailRequest { public string OldEmail { get; set; } public string NewEmail { get; set; } }
        private sealed class DeleteAlertEmailRequest { public string Email { get; set; } }

        private sealed class ListAlertEmailResponse { public string[] Items { get; set; } = Array.Empty<string>(); }
        #endregion
    }
}
