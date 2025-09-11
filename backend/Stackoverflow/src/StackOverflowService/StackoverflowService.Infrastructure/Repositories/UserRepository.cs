using Azure.Data.Tables;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Domain.Repositories;
using StackoverflowService.Infrastructure.Tables.Entities;
using StackoverflowService.Infrastructure.Tables.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Infrastructure.Mapping;

#nullable enable

namespace StackoverflowService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TableClient _users;

        public UserRepository(ITableContext ctx) { _users = ctx.Users; }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var filter = TableClient.CreateQueryFilter<UserEntity>(
                e => e.PartitionKey == "USR" && e.Email == email
            );

            await foreach (var _ in _users.QueryAsync<UserEntity>(
                filter, maxPerPage: 1, cancellationToken: cancellationToken
            ))
            {
                return true;
            }

            return false;
        }

        public async Task<User?> GetAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var resp = await _users.GetEntityAsync<UserEntity>("USR", userId, cancellationToken: cancellationToken);
                return resp.Value.ToDomain();
            }
            catch (Azure.RequestFailedException) { return null; }
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
            => await _users.AddEntityAsync(user.ToTable(), cancellationToken);

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
            => await _users.UpsertEntityAsync(user.ToTable(), TableUpdateMode.Replace, cancellationToken);

    }
}
