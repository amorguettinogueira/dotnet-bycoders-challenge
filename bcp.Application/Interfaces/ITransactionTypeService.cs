using bcp.Core.Models;

namespace bcp.Application.Interfaces;

public interface ITransactionTypeService
{
    Task<List<TransactionType>> GetAllAsync(CancellationToken cancellationToken);
}