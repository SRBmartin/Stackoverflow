#nullable enable

using StackoverflowService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace StackoverflowService.Domain.Repositories
{
    public interface IVoteRepository
    {
        Task<Vote?> GetAsync(string targetId, string voteId, CancellationToken cancellationToken);
        Task AddAsync(Vote vote, CancellationToken cancellationToken);
        Task UpsertAsync(Vote vote, CancellationToken cancellationToken);
        Task DeleteAsync(Vote vote, CancellationToken cancellationToken);
        Task<IReadOnlyList<Vote>> ListByAnswerAsync(string answerId, int take, CancellationToken cancellationToken);
        Task<Vote?> GetUserVoteForAnswerAsync(string answerId, string userId, CancellationToken cancellationToken);
        Task<(int Up, int Down)> CountByAnswerAsync(string answerId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Vote>> ListByQuestionAsync(string questionId, int take, CancellationToken cancellationToken);
        Task<(int Up, int Down)> CountByQuestionAsync(string questionId, CancellationToken cancellationToken);
        Task<Vote?> GetUserVoteForQuestionAsync(string questionId, string userId, CancellationToken cancellationToken);
    }
}
