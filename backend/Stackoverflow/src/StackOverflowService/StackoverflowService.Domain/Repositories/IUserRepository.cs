using StackoverflowService.Domain.Entities;
using System.Threading.Tasks;
using System.Threading;

#nullable enable

namespace StackoverflowService.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetAsync(string userId, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
    }
}
