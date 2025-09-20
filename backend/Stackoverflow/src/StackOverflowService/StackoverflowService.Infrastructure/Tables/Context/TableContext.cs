using Azure.Data.Tables;
using StackoverflowService.Infrastructure.Storage;
using StackoverflowService.Infrastructure.Tables.Interfaces;

namespace StackoverflowService.Infrastructure.Tables.Context
{
    public sealed class TableContext : ITableContext
    {
        private readonly TableServiceClient _svc;
        public TableClient Users { get; }
        public TableClient Questions { get; }
        public TableClient Answers { get; }
        public TableClient Votes { get; }
        public TableClient FinalEmails { get; }

        public TableContext()
        {
            _svc = new TableServiceClient(StorageConnection.Get());

            Users = _svc.GetTableClient(TableNames.Users);
            Questions = _svc.GetTableClient(TableNames.Questions);
            Answers = _svc.GetTableClient(TableNames.Answers);
            Votes = _svc.GetTableClient(TableNames.Votes);
            FinalEmails = _svc.GetTableClient(TableNames.FinalEmails);

            Users.CreateIfNotExists();
            Questions.CreateIfNotExists();
            Answers.CreateIfNotExists();
            Votes.CreateIfNotExists();
            FinalEmails.CreateIfNotExists();
        }

    }
}
