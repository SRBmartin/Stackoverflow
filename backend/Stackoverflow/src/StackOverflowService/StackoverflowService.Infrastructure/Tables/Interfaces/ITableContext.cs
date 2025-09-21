using Azure.Data.Tables;

namespace StackoverflowService.Infrastructure.Tables.Interfaces
{
    public interface ITableContext
    {
        TableClient Users { get; }
        TableClient Questions { get; }
        TableClient Answers { get; }
        TableClient Votes { get; }
        TableClient FinalEmails { get; }
    }
}
