using StackoverflowService.Domain.Repositories;
using StackoverflowService.Infrastructure.Email;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NotificationService.DTOs;
using StackoverflowService.Domain.Entities;

namespace NotificationService.Processing
{
    public class FinalAnswerNotifier : IFinalAnswerNotifier
    {
        private readonly IAnswerRepository _answers;
        private readonly IEmailClient _email;
        private readonly IFinalEmailRepository _finalEmailRepository;

        private const int Concurrency = 4;
        private const int MaxAttempts = 3;

        public FinalAnswerNotifier(
            IAnswerRepository answers,
            IEmailClient email,
            IFinalEmailRepository finalEmailRepository)
        {
            _answers = answers;
            _email = email;
            _finalEmailRepository = finalEmailRepository;
        }

        public async Task<FinalAnswerNotifyResult> NotifyContributorsAsync(string questionId, CancellationToken cancellationToken)
        {
            var finalAnswer = await _answers.GetFinalByQuestionAsync(questionId, cancellationToken);
            if (finalAnswer == null)
            {
                Trace.TraceWarning($"[Notify] No final answer found for question '{questionId}'.");
                return new FinalAnswerNotifyResult
                {
                    AnswerId = string.Empty,
                    SentCount = 0,
                    AllSucceeded = false
                };
            }

            var allAnswers = await _answers.ListByQuestionAsync(questionId, take: 0, cancellationToken: cancellationToken);
            if (allAnswers == null || allAnswers.Count == 0)
            {
                Trace.TraceWarning($"[Notify] No answers found for question '{questionId}'.");
                return new FinalAnswerNotifyResult
                {
                    AnswerId = string.Empty,
                    SentCount = 0,
                    AllSucceeded = false
                };
            }

            var recipientIds = allAnswers
                .Select(t => t.UserId)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .ToArray();

            if (recipientIds.Length == 0)
            {
                Trace.TraceWarning($"[Notify] No users found for question's '{questionId}' answers.");
                return new FinalAnswerNotifyResult
                {
                    AnswerId = string.Empty,
                    SentCount = 0,
                    AllSucceeded = false
                };
            }

            var gate = new SemaphoreSlim(Concurrency);
            var tasks = recipientIds.Select(t => SendWithRetriesAsync(t, questionId, gate, cancellationToken));
            var results = await Task.WhenAll(tasks);

            var sentCount = results.Count(ok => ok);
            var allSucceeded = results.All(ok => ok);

            var entry = new FinalEmail(finalAnswer.Id, sentCount, DateTimeOffset.UtcNow);
            await _finalEmailRepository.AddAsync(entry, cancellationToken);
            Trace.TraceInformation($"[Notify] FinalEmails logged: answerId={finalAnswer.Id}, sentCount={sentCount}, allSucceeded={allSucceeded}.");

            return new FinalAnswerNotifyResult
            {
                AnswerId = finalAnswer.Id,
                SentCount = sentCount,
                AllSucceeded = allSucceeded
            };
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
