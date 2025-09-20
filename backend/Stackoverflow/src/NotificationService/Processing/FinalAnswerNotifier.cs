using StackoverflowService.Domain.Repositories;
using StackoverflowService.Infrastructure.Email;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Processing
{
    public class FinalAnswerNotifier : IFinalAnswerNotifier
    {
        private readonly IAnswerRepository _answers;
        private readonly IEmailClient _email;

        private const int Concurrency = 4;
        private const int MaxAttempts = 3;

        public FinalAnswerNotifier(IAnswerRepository answers, IEmailClient email)
        {
            _answers = answers;
            _email = email;
        }

        public async Task<bool> NotifyContributorsAsync(string questionId, CancellationToken cancellationToken)
        {
            var allAnswers = await _answers.ListByQuestionAsync(questionId, take: 0, cancellationToken: cancellationToken);
            if (allAnswers == null || allAnswers.Count == 0) return true;

            var recipientIds = allAnswers
                .Select(t => t.UserId)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .ToArray();

            if (recipientIds.Length == 0) return true;

            var gate = new SemaphoreSlim(Concurrency);
            var tasks = recipientIds.Select(t => SendWithRetriesAsync(t, questionId, gate, cancellationToken));
            var results = await Task.WhenAll(tasks);

            return results.All(t => t);
        }

        #region Helpers

        private async Task<bool> SendWithRetriesAsync(string userId, string questionId, SemaphoreSlim gate, CancellationToken cancellationToken)
        {
            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    await gate.WaitAsync(cancellationToken);
                    var ok = await _email.SendFinalAnswerEmail(userId, questionId, cancellationToken);
                    if (ok) return true;

                    Trace.TraceWarning($"[Email] Non-success (attempt {attempt}) user='{userId}', q='{questionId}'.");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"[Email] Send failed (attempt {attempt}) user='{userId}', q='{questionId}': {ex.Message}");
                }
                finally
                {
                    gate.Release();
                }

                await Task.Delay(TimeSpan.FromMilliseconds(250 * attempt * attempt), cancellationToken);
            }

            return false;
        }

        #endregion

    }
}
