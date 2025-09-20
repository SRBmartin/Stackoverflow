using NotificationService.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Processing
{
    public interface IFinalAnswerNotifier
    {
        Task<FinalAnswerNotifyResult> NotifyContributorsAsync(string questionId, CancellationToken cancellationToken);
    }
}
