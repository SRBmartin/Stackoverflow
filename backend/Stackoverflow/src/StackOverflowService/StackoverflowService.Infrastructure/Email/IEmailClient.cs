using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Infrastructure.Email
{
    public interface IEmailClient
    {
        Task<bool> SendFinalAnswerEmail(string recipientUserId, string questionId, CancellationToken cancellationToken);
    }
}
