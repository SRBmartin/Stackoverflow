using Autofac;
using Azure;
using Azure.Storage.Queues;
using NotificationService.Processing;
using NotificationService.Queues;
using StackoverflowService.Infrastructure.Queues;
using StackoverflowService.Infrastructure.Storage;

namespace NotificationService.Composition
{
    public class NotificationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Queue clients (singletons)
            builder.Register(ctx =>
            {
                var cs = StorageConnection.Get();
                var q = new QueueClient(cs, QueueNames.FinalAnswers);
                var poison = new QueueClient(cs, $"{QueueNames.FinalAnswers}-poison");

                try { q.CreateIfNotExists(); poison.CreateIfNotExists(); }
                catch (RequestFailedException) { /* Azurite races are ok/transient */ }

                return new FinalAnswersQueues(q, poison);
            })
            .AsSelf()
            .SingleInstance();

            builder.RegisterType<FinalAnswerNotifier>()
                   .As<IFinalAnswerNotifier>()
                   .InstancePerDependency();

            builder.RegisterType<QueueDequeueService>()
                   .AsSelf()
                   .SingleInstance();
        }
    }
}
