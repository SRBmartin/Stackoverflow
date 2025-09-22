using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using NotificationService.Composition;
using StackoverflowService.Infrastructure.Composition;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private IContainer _container;

        private HttpListener _healthListener;
        private Task _healthTask;

        public override void Run()
        {
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                using (var scope = _container.BeginLifetimeScope())
                {
                    var svc = scope.Resolve<QueueDequeueService>();
                    svc.RunAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
                }
            }
            catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested) { }
            catch (Exception ex)
            {
                Trace.TraceError($"[Notification Service]: WorkerRole.Run fatal error: {ex}");
                throw;
            }
        }

        public override bool OnStart()
        {
            // Use TLS 1.2 for Service Bus connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            var cb = new ContainerBuilder();

            cb.RegisterModule(new InfrastructureModule());
            cb.RegisterModule(new NotificationModule());

            _container = cb.Build();

            StartHealthEndpoint();

            bool result = base.OnStart();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            try { _healthListener?.Stop(); } catch { /* ignore */ }
            try { _healthTask?.Wait(TimeSpan.FromSeconds(3)); } catch { /* ignore */ }

            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

        #region Health Monitoring

        private void StartHealthEndpoint()
        {
            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Health"].IPEndpoint;

            var host = endpoint.Address.AddressFamily == AddressFamily.InterNetworkV6 
                ? $"[{endpoint.Address}]"
                : endpoint.Address.ToString();

            var prefix = $"http://{host}:{endpoint.Port}/";

            _healthListener = new HttpListener();
            _healthListener.Prefixes.Add(prefix);
            _healthListener.Start();

            _healthTask = Task.Run(() => HealthLoopAsync(cancellationTokenSource.Token));
        }

        private async Task HealthLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                HttpListenerContext ctx = null;
                try
                {
                    ctx = await _healthListener.GetContextAsync();

                    var path = ctx.Request.Url.AbsolutePath.TrimEnd('/').ToLowerInvariant();
                    if (path == "/health-monitoring")
                    {
                        ctx.Response.StatusCode = 200;
                    }
                    else
                    {
                        ctx.Response.StatusCode = 404;
                    }

                    ctx.Response.Close();
                }
                catch (ObjectDisposedException)
                {
                    //Dispoding listener during shutdown process
                }
                catch (HttpListenerException)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                }
                catch
                {
                    try
                    {
                        if (ctx != null && ctx.Response.OutputStream.CanWrite)
                        {
                            ctx.Response.StatusCode = 500;
                            ctx.Response.Close();
                        }
                    }
                    catch { /* ignore */ }
                }
            }
        }

        #endregion

    }
}
