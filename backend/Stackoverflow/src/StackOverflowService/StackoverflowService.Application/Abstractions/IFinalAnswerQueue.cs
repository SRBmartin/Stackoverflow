using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Application.Abstractions
{
    public interface IFinalAnswerQueue
    {
        Task EnqueueAsync(string questionId, CancellationToken cancellationToken);
    }
}
