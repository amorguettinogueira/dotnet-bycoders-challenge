using bcp.Application.DTOs;
using Refit;

namespace bcp.UI.Services;

public interface ITransactionFileApi
{
    [Get("/api/files")]
    Task<List<FileSummary>> GetFilesAsync();

    [Get("/api/files/{id}/summary")]
    Task<List<StoreAggregation>> GetAggregatedDataAsync(int id);
}
