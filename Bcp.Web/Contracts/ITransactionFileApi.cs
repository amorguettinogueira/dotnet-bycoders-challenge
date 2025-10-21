using Bcp.Application.DTOs;
using Refit;

namespace Bcp.Web.Contracts;

public interface ITransactionFileApi
{
    [Get("/api/files")]
    Task<List<Application.DTOs.File>> GetFilesAsync();

    [Get("/api/files/{id}/summary")]
    Task<FileSummary> GetAggregatedDataAsync(int id);

    [Multipart]
    [Post("/api/files/upload")]
    Task<ApiResponse<string>> UploadFileAsync([AliasAs("file")] StreamPart file);
}
