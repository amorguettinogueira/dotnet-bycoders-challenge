using Refit;
using Bcp.Application.DTOs;

namespace Bcp.Worker.Contracts;

public interface INotificationsApi
{
    [Post("/api/notifications/file-processed")]
    Task<ApiResponse<object>> FileProcessedAsync([Body] FileProcessed dto);
}