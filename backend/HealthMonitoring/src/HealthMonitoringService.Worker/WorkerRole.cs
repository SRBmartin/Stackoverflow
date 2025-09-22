using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using HealthMonitoringService.Infrastructure.Composition;
using HealthMonitoringService.Worker.Composition;
using HealthMonitoringService.Worker.Endpoints;
using HealthMonitoringService.Worker.Monitoring.Interfaces;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace HealthMonitoringService.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private IContainer _container;

        private AlertEmailHttpServer _alertApi;

        public override void Run()
        {
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                using (var scope = _container.BeginLifetimeScope())
                {
                    var monitor = scope.Resolve<IMonitoringService>();
                    monitor.RunAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
                }
            }
            catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested) { }
            catch (Exception ex)
            {
                Trace.TraceError("[HealthMonitoringService.Worker] Fatal error: " + ex);
                throw;
            }
            finally
            {
                runCompleteEvent.Set();
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

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(new InfrastructureModule());
            containerBuilder.RegisterModule(new WorkerModule());

            _container = containerBuilder.Build();

            StartAlertApiEndpoint();

            bool result = base.OnStart();

            Trace.TraceInformation("HealthMonitoringService.Worker has started.");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService.Worker is stopping.");

            try { cancellationTokenSource?.Cancel(); } catch { }

            runCompleteEvent.WaitOne(TimeSpan.FromSeconds(10));

            _container?.Dispose();

            base.OnStop();

            Trace.TraceInformation("HealthMonitoringService.Worker has stopped.");
        }

        private void StartAlertApiEndpoint()
        {
            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["AlertApi"].IPEndpoint;
            _alertApi = new AlertEmailHttpServer(_container);
            _alertApi.Start(endpoint);
        }

    }
}
