using bcp.Application.DTOs;

namespace bcp.Application.Interfaces;

public interface ITransactionFileService
{
    Task<List<FileSummary>> GetFileSummariesAsync();
    Task<List<StoreAggregation>> GetStoreAggregationsAsync(int fileId);
}