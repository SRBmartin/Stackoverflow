using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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

            bool result = base.OnStart();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

    }
}
