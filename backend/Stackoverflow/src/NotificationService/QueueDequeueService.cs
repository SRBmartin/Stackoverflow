using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using StackoverflowService.Infrastructure.Queues;
using StackoverflowService.Infrastructure.Storage;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService
{
    public sealed class QueueDequeueService
    {
        private const int MaxBatch = 16;
        private static readonly TimeSpan VisibilityTimeout = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan MinBackoff = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(10);
        private const int PoisonThreshold = 5;

        private readonly QueueClient _queue;
        private readonly QueueClient _poisonQueue;
        private readonly string _instanceId;

        public QueueDequeueService()
        {
            _instanceId = GetInstanceId();

            var cs = StorageConnection.Get();

            var mainQueueName = QueueNames.FinalAnswers;
            _queue = new QueueClient(cs, mainQueueName);

            var poisonName = $"{mainQueueName}-poison";
            _poisonQueue = new QueueClient(cs, poisonName);

            try
            {
                _queue.CreateIfNotExists();
                _poisonQueue.CreateIfNotExists();
            }
            catch (RequestFailedException)
            {
                //ignore azurite races/transients
            }

            Trace.TraceInformation($"[Queue][{_instanceId}] Initialized. main='{_queue.Name}', poison='{_poisonQueue.Name}'.");
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var backoff = MinBackoff;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    QueueMessage[] messages = Array.Empty<QueueMessage>();

                    try
                    {
                        var response = await _queue.ReceiveMessagesAsync(maxMessages: MaxBatch, visibilityTimeout: VisibilityTimeout, cancellationToken: cancellationToken);

                        messages = response.Value ?? Array.Empty<QueueMessage>();
                        if (messages.Length > 0)
                            Trace.TraceInformation($"[Queue][{_instanceId}] Received {messages.Length} message(s).");

                    }
                    catch (RequestFailedException ex)
                    {
                        Trace.TraceWarning($"[Queue] Receive failed: {ex.Message}");
                        messages = Array.Empty<QueueMessage>();
                    }

                    if (messages.Length == 0)
                    {
                        await Task.Delay(backoff, cancellationToken);
                        backoff = TimeSpan.FromMilliseconds(Math.Min(backoff.TotalMilliseconds * 2, MaxBackoff.TotalMilliseconds));
                        continue;
                    }

                    backoff = MinBackoff;

                    await Task.WhenAll(messages.Select(m => ProcessOneAsync(m, cancellationToken)));
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"[Queue] Fatal loop error: {ex}");
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                }
            }

        }

        #region Helpers

        private async Task ProcessOneAsync(QueueMessage msg, CancellationToken ct)
        {
            // message text is the questionId (SDK base64-decodes for us)
            var questionId = (msg.MessageText ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(questionId))
            {
                await SafeDeleteAsync(msg, ct);
                Trace.TraceWarning("[Queue] Deleted empty/whitespace message.");
                return;
            }

            try
            {
                // TODO: Put your real processing here (e.g., publish a domain notification, call EmailClient, etc.)
                // For now, just trace:
                Trace.TraceInformation($"[Queue] Processing questionId='{questionId}' (DequeueCount={msg.DequeueCount})");

                // If long processing is expected, you can extend visibility:
                // await _queue.UpdateMessageAsync(msg.MessageId, msg.PopReceipt, msg.MessageText, VisibilityTimeout, ct);

                // If successful, delete it
                await SafeDeleteAsync(msg, ct);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Queue] Processing failed for questionId='{questionId}': {ex.Message}");

                // Poison after repeated failures
                if (msg.DequeueCount >= PoisonThreshold)
                {
                    try
                    {
                        await _poisonQueue.SendMessageAsync(msg.MessageText, ct);
                        await SafeDeleteAsync(msg, ct);
                        Trace.TraceWarning($"[Queue] Moved to poison queue after {msg.DequeueCount} attempts: {questionId}");
                    }
                    catch (Exception pex)
                    {
                        Trace.TraceError($"[Queue] Poison handling failed: {pex}");
                        // let it reappear after visibility timeout
                    }
                }
                // else: let it reappear after visibility timeout for retry
            }
        }

        private async Task SafeDeleteAsync(QueueMessage msg, CancellationToken ct)
        {
            try
            {
                await _queue.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, ct);
            }
            catch (RequestFailedException ex)
            {
                // benign race if another instance deleted it
                Trace.TraceWarning($"[Queue] Delete failed (likely race): {ex.Message}");
            }
        }

        private static string GetInstanceId()
        {
            try
            {
                return RoleEnvironment.IsAvailable
                    ? RoleEnvironment.CurrentRoleInstance.Id
                    : Environment.MachineName;
            }
            catch
            {
                return Environment.MachineName;
            }
        }

        #endregion

    }

}
