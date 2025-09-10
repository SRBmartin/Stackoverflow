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

#nullable enable

namespace StackoverflowService.Infrastructure.Repositories
{
    public sealed class VoteRepository : IVoteRepository
    {
        private readonly TableClient _votes;

        public VoteRepository(ITableContext ctx) { _votes = ctx.Votes; }

        public async Task<Vote?> GetAsync(string answerId, string voteId, CancellationToken ct)
        {
            try
            {
                var resp = await _votes.GetEntityAsync<VoteEntity>(answerId, voteId, cancellationToken: ct);
                return resp.Value.ToDomain();
            }
            catch (RequestFailedException) { return null; }
        }

        public async Task AddAsync(Vote v, CancellationToken ct)
            => await _votes.AddEntityAsync(v.ToTable(), ct);

        public async Task<IReadOnlyList<Vote>> ListByAnswerAsync(string answerId, int take, CancellationToken ct)
        {
            var filter = TableClient.CreateQueryFilter<VoteEntity>(e => e.PartitionKey == answerId);
            var list = new List<Vote>(capacity: take > 0 ? take : 16);

            await foreach (var e in _votes.QueryAsync<VoteEntity>(filter, cancellationToken: ct))
            {
                list.Add(e.ToDomain());
                if (take > 0 && list.Count >= take) break;
            }
            return list;
        }

        public async Task<Vote?> GetUserVoteForAnswerAsync(string answerId, string userId, CancellationToken ct)
        {
            var filter = TableClient.CreateQueryFilter<VoteEntity>(e => e.PartitionKey == answerId && e.UserId == userId);
            await foreach (var e in _votes.QueryAsync<VoteEntity>(filter, maxPerPage: 1, cancellationToken: ct))
                return e.ToDomain();

            return null;
        }

        public async Task UpsertAsync(Vote v, CancellationToken ct)
            => await _votes.UpsertEntityAsync(v.ToTable(), TableUpdateMode.Replace, ct);
    }
}
