using Azure;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Infrastructure.Queues.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Infrastructure.Queues.Services
{
    public class FinalAnswerQueue : IFinalAnswerQueue
    {
        private readonly IQueueContext _queueContext;

        public FinalAnswerQueue(IQueueContext queueContext)
        {
            _queueContext = queueContext;
        }

        public async Task EnqueueAsync(string questionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(questionId))
                throw new ArgumentException("questionId is required.", nameof(questionId));

            try
            {
                await _queueContext.FinalAnswers.SendMessageAsync(questionId, cancellationToken);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException($"Failed to enqueue message for question '{questionId}'.", ex);
            }

        }
    }
}
