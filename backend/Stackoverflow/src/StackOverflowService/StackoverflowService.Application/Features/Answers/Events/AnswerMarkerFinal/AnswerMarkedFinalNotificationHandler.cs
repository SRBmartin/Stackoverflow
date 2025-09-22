using MediatR;
using StackoverflowService.Application.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Features.Answers.Events.AnswerMarkerFinal
{
    public class AnswerMarkedFinalNotificationHandler : INotificationHandler<AnswerMarkedFinalNotification>
    {
        private readonly IFinalAnswerQueue _finalAnswerQueue;

        public AnswerMarkedFinalNotificationHandler(IFinalAnswerQueue finalAnswerQueue)
        {
            _finalAnswerQueue = finalAnswerQueue;
        }

        public async Task Handle(AnswerMarkedFinalNotification notification, CancellationToken cancellationToken)
        {
            await _finalAnswerQueue.EnqueueAsync(notification.QuestionId, cancellationToken);
        }

    }
}
