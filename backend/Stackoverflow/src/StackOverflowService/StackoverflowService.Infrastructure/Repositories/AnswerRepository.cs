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
    public sealed class AnswerRepository : IAnswerRepository
    {
        private readonly TableClient _answers;

        public AnswerRepository(ITableContext ctx) { _answers = ctx.Answers; }

        public async Task<Answer?> GetAsync(string questionId, string answerId, CancellationToken cancellationToken)
        {
            try
            {
                var resp = await _answers.GetEntityAsync<AnswerEntity>(questionId, answerId, cancellationToken: cancellationToken);
                return resp.Value.ToDomain();
            }
            catch (Azure.RequestFailedException) { return null; }
        }

        public async Task AddAsync(Answer a, CancellationToken cancellationToken)
            => await _answers.AddEntityAsync(a.ToTable(), cancellationToken);

        public async Task<IReadOnlyList<Answer>> ListByQuestionAsync(string questionId, int take, CancellationToken cancellationToken)
        {
            var filter = TableClient.CreateQueryFilter<AnswerEntity>(e => e.PartitionKey == questionId);
            var list = new List<Answer>(capacity: take > 0 ? take : 16);
            await foreach (var e in _answers.QueryAsync<AnswerEntity>(filter, cancellationToken: cancellationToken))
            {
                list.Add(e.ToDomain());
                if (take > 0 && list.Count >= take) break;
            }
            return list;
        }

        public async Task UpdateAsync(Answer a, CancellationToken cancellationToken)
            => await _answers.UpsertEntityAsync(a.ToTable(), TableUpdateMode.Replace, cancellationToken);
    }
}
