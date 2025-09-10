#nullable enable

using StackoverflowService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace StackoverflowService.Domain.Repositories
{
    public interface IAnswerRepository
    {
        Task<Answer?> GetAsync(string questionId, string answerId, CancellationToken cancellationToken);
        Task AddAsync(Answer answer, CancellationToken cancellationToken);
        Task<IReadOnlyList<Answer>> ListByQuestionAsync(string questionId, int take, CancellationToken cancellationToken);
        Task UpdateAsync(Answer answer, CancellationToken cancellationToken);
    }
}
