using Bcp.Web.Configuration;
using Bcp.Web.Contracts;
using Bcp.Web.Models;
using Microsoft.Extensions.Options;
using Refit;

namespace Bcp.Web.Services;

public class FileUploadService(ITransactionFileApi api,
                               IOptions<FileUploadOptions> options) : IFileUploadService
{
    public async Task<UploadResultModel> UploadFileAsync(IFormFile file)
    {
        try
        {
            if (file.Length == 0)
            {
                return UploadResultModel.Failure("File is empty");
            }

            if (file.Length > options.Value.MaxFileSize)
            {
                return UploadResultModel.Failure("File exceeds maximum allowed size");
            }

            var extension = Path.GetExtension(file.FileName);
            if (!options.Value.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return UploadResultModel.Failure("Invalid file type");
            }

            using var stream = file.OpenReadStream();
            var streamPart = new StreamPart(stream, file.FileName, "text/plain");

            var response = await api.UploadFileAsync(streamPart);

            return response.IsSuccessStatusCode
                ? UploadResultModel.Success()
                : UploadResultModel.Failure(response.Error?.Message ?? "Upload failed");
        }
        catch (ApiException ex)
        {
            return UploadResultModel.Failure($"API Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return UploadResultModel.Failure($"Error uploading file: {ex.Message}");
        }
    }
}