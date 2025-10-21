using Bcp.Application.DTOs;

namespace Bcp.Application.Contracts;

public interface ITransactionFileService
{
    Task<List<DTOs.File>> GetFileSummariesAsync();
    Task<FileSummary> GetStoreAggregationsAsync(int fileId);
}