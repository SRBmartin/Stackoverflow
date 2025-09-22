using Azure.Storage.Queues;

namespace NotificationService.Queues
{
    public class FinalAnswersQueues
    {
        public QueueClient Main { get; }
        public QueueClient Poison { get; }

        public FinalAnswersQueues(QueueClient main, QueueClient poison)
        {
            Main = main;
            Poison = poison;
        }

    }
}
