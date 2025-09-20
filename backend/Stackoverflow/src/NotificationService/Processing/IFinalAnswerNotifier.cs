using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Processing
{
    public interface IFinalAnswerNotifier
    {
        Task<bool> NotifyContributorsAsync(string questionId, CancellationToken cancellationToken);
    }
}
