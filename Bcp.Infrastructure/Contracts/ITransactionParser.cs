using Bcp.Domain.Models;
using Bcp.Infrastructure.Models;

namespace Bcp.Infrastructure.Contracts;

public interface ITransactionParser
{
    Task<List<Transaction>> ParseBatchAsync(TransactionRecord[] records, CancellationToken cancellationToken);
}
