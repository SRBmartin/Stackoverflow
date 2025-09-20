using Azure;
using Azure.Storage.Queues.Models;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NotificationService.Processing;
using NotificationService.Queues;

namespace NotificationService
{
    public sealed class QueueDequeueService
    {
        private const int MaxBatch = 16;
        private static readonly TimeSpan VisibilityTimeout = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan MinBackoff = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(10);
        private const int PoisonThreshold = 5;

        private readonly FinalAnswersQueues _queues;
        private readonly IFinalAnswerNotifier _notifier;
        private readonly string _instanceId;

        public QueueDequeueService(FinalAnswersQueues queues, IFinalAnswerNotifier notifier)
        {
            _queues = queues;
            _notifier = notifier;
            _instanceId = GetInstanceId();

            Trace.TraceInformation($"[Queue][{_instanceId}] Initialized. main='{_queues.Main.Name}', poison='{_queues.Poison.Name}'.");
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var backoff = MinBackoff;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    QueueMessage[] messages;

                    try
                    {
                        var response = await _queues.Main.ReceiveMessagesAsync(
                            maxMessages: MaxBatch,
                            visibilityTimeout: VisibilityTimeout,
                            cancellationToken: cancellationToken
                        );

                        messages = response.Value ?? Array.Empty<QueueMessage>();
                        if (messages.Length > 0)
                            Trace.TraceInformation($"[Queue][{_instanceId}] Received {messages.Length} message(s).");
                    }
                    catch (RequestFailedException ex)
                    {
                        Trace.TraceWarning($"[Queue][{_instanceId}] Receive failed: {ex.Message}");
                        messages = Array.Empty<QueueMessage>();
                    }

                    if (messages.Length == 0)
                    {
                        await Task.Delay(backoff, cancellationToken);
                        backoff = TimeSpan.FromMilliseconds(Math.Min(backoff.TotalMilliseconds * 2, MaxBackoff.TotalMilliseconds));
                        continue;
                    }

                    backoff = MinBackoff;

                    foreach (var m in messages)
                    {
                        await ProcessOneAsync(m, cancellationToken);
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"[Queue][{_instanceId}] Fatal loop error: {ex}");
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                }
            }
        }

        private async Task ProcessOneAsync(QueueMessage msg, CancellationToken cancellationToken)
        {
            var questionId = (msg.MessageText ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(questionId))
            {
                await SafeDeleteAsync(msg, cancellationToken);
                Trace.TraceWarning($"[Queue][{_instanceId}] Deleted empty/whitespace message.");
                return;
            }

            try
            {
                Trace.TraceInformation($"[Queue][{_instanceId}] Processing questionId='{questionId}' (DequeueCount={msg.DequeueCount})");

                var allOk = await _notifier.NotifyContributorsAsync(questionId, cancellationToken);

                if (allOk)
                {
                    await SafeDeleteAsync(msg, cancellationToken);
                    return;
                }

                throw new InvalidOperationException("One or more email sends failed.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[Queue][{_instanceId}] Processing failed for questionId='{questionId}': {ex.Message}");

                if (msg.DequeueCount + 1 >= PoisonThreshold)
                {
                    try
                    {
                        await _queues.Poison.SendMessageAsync(msg.MessageText, cancellationToken);
                        await SafeDeleteAsync(msg, cancellationToken);
                        Trace.TraceWarning($"[Queue][{_instanceId}] Moved to poison after {msg.DequeueCount + 1} attempts: {questionId}");
                    }
                    catch (Exception pex)
                    {
                        Trace.TraceError($"[Queue][{_instanceId}] Poison handling failed: {pex}");
                    }
                }
            }
        }

        private async Task SafeDeleteAsync(QueueMessage msg, CancellationToken ct)
        {
            try
            {
                await _queues.Main.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, ct);
            }
            catch (RequestFailedException ex)
            {
                Trace.TraceWarning($"[Queue][{_instanceId}] Delete failed (likely race): {ex.Message}");
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
    }
}
