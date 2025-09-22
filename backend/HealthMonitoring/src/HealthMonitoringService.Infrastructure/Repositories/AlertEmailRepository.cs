using Azure;
using Azure.Data.Tables;
using HealthMonitoringService.Domain.Entities;
using HealthMonitoringService.Domain.Repositories;
using HealthMonitoringService.Infrastructure.Mapping;
using HealthMonitoringService.Infrastructure.Tables.Entities;
using HealthMonitoringService.Infrastructure.Tables.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService.Infrastructure.Repositories
{
    public class AlertEmailRepository : IAlertEmailRepository
    {
        private readonly TableClient _alertEmails;

        public AlertEmailRepository(ITableContext tableContext)
        {
            _alertEmails = tableContext.AlertEmails;
        }

        public async Task AddAsync(AlertEmail email, CancellationToken cancellationToken)
        {
            var entity = email.ToTable();
            
            await _alertEmails.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
        {
            var keys = AlertEmailEntity.KeysFromEmail(email);
            try
            {
                await _alertEmails.GetEntityAsync<AlertEmailEntity>(keys.pk, keys.rk, cancellationToken: cancellationToken);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return false;
            }
        }

        public async Task<IReadOnlyList<AlertEmail>> GetAllAsync(CancellationToken cancellationToken)
        {
            var list = new List<AlertEmail>();
            await foreach (var e in _alertEmails.QueryAsync<AlertEmailEntity>(cancellationToken: cancellationToken))
            {
                list.Add(e.ToDomain());
            }
            return list;
        }

        public async Task RemoveAsync(string email, CancellationToken cancellationToken)
        {
            var keys = AlertEmailEntity.KeysFromEmail(email);
            try
            {
                await _alertEmails.DeleteEntityAsync(keys.pk, keys.rk, cancellationToken: cancellationToken);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                //ignore
            }
        }
    }
}
