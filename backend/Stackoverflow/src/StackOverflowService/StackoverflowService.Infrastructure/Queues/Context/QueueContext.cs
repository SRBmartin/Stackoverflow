using Azure;
using Azure.Storage.Queues;
using StackoverflowService.Infrastructure.Queues.Interfaces;
using StackoverflowService.Infrastructure.Storage;
using System.Configuration;

namespace StackoverflowService.Infrastructure.Queues.Context
{
    public class QueueContext : IQueueContext
    {
        private readonly QueueServiceClient _svc;
        public QueueClient FinalAnswers { get; }

        public QueueContext()
        {
            var connectionString = StorageConnection.Get();
            _svc = new QueueServiceClient(connectionString);

            var finalAnswersName = ConfigurationManager.AppSettings["FinalAnswerQueue"] ?? QueueNames.FinalAnswers;

            FinalAnswers = _svc.GetQueueClient(finalAnswersName);

            try
            {
                FinalAnswers.CreateIfNotExists();
            }
            catch (RequestFailedException)
            {
                //ignore emulator races/transients
            }
        }

    }
}
