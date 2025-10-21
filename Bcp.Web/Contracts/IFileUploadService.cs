using Bcp.Web.Models;

namespace Bcp.Web.Contracts;

public interface IFileUploadService
{
    Task<UploadResultModel> UploadFileAsync(IFormFile file);
}
