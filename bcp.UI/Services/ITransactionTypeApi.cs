using bcp.Core.Models;
using Refit;

namespace bcp.UI.Services;

public interface ITransactionTypeApi
{
    [Get("/api/TransactionTypes")]
    Task<List<TransactionType>> GetAllAsync();
}
