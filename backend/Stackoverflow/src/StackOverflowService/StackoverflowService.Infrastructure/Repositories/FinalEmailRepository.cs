using Azure.Data.Tables;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Domain.Repositories;
using StackoverflowService.Infrastructure.Mapping;
using StackoverflowService.Infrastructure.Tables.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace StackoverflowService.Infrastructure.Repositories
{
    public class FinalEmailRepository : IFinalEmailRepository
    {
        private readonly TableClient _ctx;

        public FinalEmailRepository(ITableContext ctx)
        {
            _ctx = ctx.FinalEmails;
        }

        public async Task AddAsync(FinalEmail email, CancellationToken cancellationToken)
        {
            var entity = email.ToTable();
            await _ctx.AddEntityAsync(entity, cancellationToken);
        }

    }
}
