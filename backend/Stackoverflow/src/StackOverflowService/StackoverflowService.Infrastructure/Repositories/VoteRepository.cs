using Azure;
using Azure.Data.Tables;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Infrastructure.Tables.Entities;
using StackoverflowService.Infrastructure.Tables.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Infrastructure.Mapping;
using StackoverflowService.Domain.Repositories;
using StackoverflowService.Domain.Enums;

#nullable enable

namespace StackoverflowService.Infrastructure.Repositories
{
    public sealed class VoteRepository : IVoteRepository
    {
        private readonly TableClient _votes;

        public VoteRepository(ITableContext ctx) { _votes = ctx.Votes; }

        public async Task<Vote?> GetAsync(string targetId, string voteId, CancellationToken cancellationToken)
        {
            try
            {
                var resp = await _votes.GetEntityAsync<VoteEntity>(targetId, voteId, cancellationToken: cancellationToken);
                return resp.Value.ToDomain();
            }
            catch (RequestFailedException) { return null; }
        }

        public async Task AddAsync(Vote v, CancellationToken cancellationToken)
            => await _votes.AddEntityAsync(v.ToTable(), cancellationToken);

        public async Task UpsertAsync(Vote v, CancellationToken cancellationToken)
            => await _votes.UpsertEntityAsync(v.ToTable(), TableUpdateMode.Replace, cancellationToken);

        public Task<IReadOnlyList<Vote>> ListByAnswerAsync(string answerId, int take, CancellationToken ct)
            => ListByPartitionAsync(answerId, take, ct);

        public Task<(int Up, int Down)> CountByAnswerAsync(string answerId, CancellationToken ct)
            => CountByPartitionAsync(answerId, ct);

        public Task<Vote?> GetUserVoteForAnswerAsync(string answerId, string userId, CancellationToken ct)
            => FirstByUserAsync(answerId, userId, ct);

        public Task<IReadOnlyList<Vote>> ListByQuestionAsync(string questionId, int take, CancellationToken ct)
            => ListByPartitionAsync(questionId, take, ct);

        public Task<(int Up, int Down)> CountByQuestionAsync(string questionId, CancellationToken ct)
            => CountByPartitionAsync(questionId, ct);

        public Task<Vote?> GetUserVoteForQuestionAsync(string questionId, string userId, CancellationToken ct)
            => FirstByUserAsync(questionId, userId, ct);

        #region Helpers

        private static bool IsUp(string t) => t == "+";
        private async Task<(int Up, int Down)> CountByPartitionAsync(string partitionKey, CancellationToken ct)
        {
            var filter = TableClient.CreateQueryFilter<VoteEntity>(e => e.PartitionKey == partitionKey);
            int up = 0, down = 0;
            await foreach (var e in _votes.QueryAsync<VoteEntity>(filter, cancellationToken: ct))
            {
                if (IsUp(e.Type)) up++; else down++;
            }
            return (up, down);
        }

        private async Task<IReadOnlyList<Vote>> ListByPartitionAsync(string partitionKey, int take, CancellationToken ct)
        {
            var filter = TableClient.CreateQueryFilter<VoteEntity>(e => e.PartitionKey == partitionKey);
            var list = new List<Vote>(take > 0 ? take : 32);
            await foreach (var e in _votes.QueryAsync<VoteEntity>(filter, cancellationToken: ct))
            {
                list.Add(e.ToDomain());
                if (take > 0 && list.Count >= take) break;
            }
            return list;
        }

        private async Task<Vote?> FirstByUserAsync(string partitionKey, string userId, CancellationToken ct)
        {
            var filter = TableClient.CreateQueryFilter<VoteEntity>(e => e.PartitionKey == partitionKey && e.UserId == userId);
            await foreach (var e in _votes.QueryAsync<VoteEntity>(filter, maxPerPage: 1, cancellationToken: ct))
                return e.ToDomain();
            return null;
        }

        #endregion

    }
}
