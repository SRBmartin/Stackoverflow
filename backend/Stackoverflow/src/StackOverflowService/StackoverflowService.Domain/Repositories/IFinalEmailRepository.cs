using StackoverflowService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Domain.Repositories
{
    public interface IFinalEmailRepository
    {
        Task AddAsync(FinalEmail email, CancellationToken cancellationToken);
    }
}
