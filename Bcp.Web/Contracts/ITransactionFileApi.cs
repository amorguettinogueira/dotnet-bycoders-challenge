using Bcp.Application.DTOs;
using Refit;

namespace Bcp.Web.Contracts;

public interface ITransactionFileApi
{
    [Get("/api/files")]
    Task<List<Application.DTOs.File>> GetFilesAsync();

    [Get("/api/files/{id}/summary")]
    Task<FileSummary> GetAggregatedDataAsync(int id);

    [Get("/api/files/{fileId}/stores/{storeId}/transactions")]
    Task<List<TransactionItem>> GetTransactionsAsync(int fileId, int storeId);

    [Multipart]
    [Post("/api/files/upload")]
    Task<ApiResponse<string>> UploadFileAsync([AliasAs("file")] StreamPart file);
}
