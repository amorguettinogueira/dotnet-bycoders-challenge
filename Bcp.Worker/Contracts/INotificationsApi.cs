using Bcp.Application.DTOs;
using Refit;

namespace Bcp.Worker.Contracts;

public interface INotificationsApi
{
    [Post("/api/notifications/file-processed")]
    Task<ApiResponse<object>> FileProcessedAsync([Body] FileProcessed dto);
}