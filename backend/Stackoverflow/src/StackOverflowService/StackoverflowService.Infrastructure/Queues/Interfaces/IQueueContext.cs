using Azure.Storage.Queues;

namespace StackoverflowService.Infrastructure.Queues.Interfaces
{
    public interface IQueueContext
    {
        QueueClient FinalAnswers { get; }
    }
}
