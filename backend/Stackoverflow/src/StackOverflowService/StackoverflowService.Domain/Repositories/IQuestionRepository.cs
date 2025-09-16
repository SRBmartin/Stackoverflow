#nullable enable

using StackoverflowService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace StackoverflowService.Domain.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question?> GetAsync(string userId, string questionId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Question>> GetAllFilteredAsync(string? titleStartsWith, CancellationToken cancellationToken);
        Task AddAsync(Question question, CancellationToken cancellationToken);
        Task<IReadOnlyList<Question>> ListByUserAsync(string userId, int take, CancellationToken cancellationToken  );
        Task UpdateAsync(Question question, CancellationToken cancellationToken);
    }
}
