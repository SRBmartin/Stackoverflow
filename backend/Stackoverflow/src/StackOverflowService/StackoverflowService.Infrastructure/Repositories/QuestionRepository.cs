using Azure.Data.Tables;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Domain.Repositories;
using StackoverflowService.Infrastructure.Tables.Entities;
using StackoverflowService.Infrastructure.Tables.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Infrastructure.Mapping;

#nullable enable

namespace StackoverflowService.Infrastructure.Repositories
{
    public sealed class QuestionRepository : IQuestionRepository
    {
        private readonly TableClient _questions;

        public QuestionRepository(ITableContext ctx) { _questions = ctx.Questions; }

        public async Task<Question?> GetAsync(string userId, string questionId, CancellationToken cancellationToken)
        {
            try
            {
                var resp = await _questions.GetEntityAsync<QuestionEntity>(userId, questionId, cancellationToken: cancellationToken);

                var e = resp.Value;
                if (e.IsDeleted) return null;
                return e.ToDomain();
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
            catch (Azure.RequestFailedException)
            {
                return null;
            }
        }

        public async Task<Question?> GetByIdAsync(string questionId, CancellationToken cancellationToken)
        {
            var filter = TableClient.CreateQueryFilter<QuestionEntity>(e => e.RowKey == questionId && e.IsDeleted == false);

            await foreach (var e in _questions.QueryAsync<QuestionEntity>(filter, maxPerPage: 1, cancellationToken: cancellationToken))
            {
                return e.ToDomain();
            }

            return null;
        }

        public async Task<IReadOnlyList<Question>> GetAllFilteredAsync(string? titleStartsWith, CancellationToken cancellationToken)
        {
            string filter = "IsDeleted eq false";

            if (!string.IsNullOrWhiteSpace(titleStartsWith) && !string.IsNullOrEmpty(titleStartsWith))
            {
                var low = EscapeOdataString(titleStartsWith);
                var high = low + "\uFFFF";
                filter += $" and Title ge '{low}' and Title lt '{high}'";
            }

            var list = new List<Question>(capacity: 128);
            await foreach (var e in _questions.QueryAsync<QuestionEntity>(filter: filter, cancellationToken: cancellationToken))
            {
                list.Add(e.ToDomain());
            }

            return list;
        }

        public async Task AddAsync(Question q, CancellationToken cancellationToken)
            => await _questions.AddEntityAsync(q.ToTable(), cancellationToken);

        public async Task<IReadOnlyList<Question>> ListByUserAsync(string userId, int take, CancellationToken cancellationToken)
        {
            var filter = TableClient.CreateQueryFilter<QuestionEntity>(e => e.PartitionKey == userId);
            var list = new List<Question>(capacity: take > 0 ? take : 16);
            await foreach (var e in _questions.QueryAsync<QuestionEntity>(filter, cancellationToken: cancellationToken))
            {
                list.Add(e.ToDomain());
                if (take > 0 && list.Count >= take) break;
            }
            return list;
        }

        public async Task UpdateAsync(Question q, CancellationToken cancellationToken)
            => await _questions.UpsertEntityAsync(q.ToTable(), TableUpdateMode.Replace, cancellationToken);

        #region Helpers
        private static string EscapeOdataString(string s) => s.Replace("'", "''");
        #endregion

    }
}
