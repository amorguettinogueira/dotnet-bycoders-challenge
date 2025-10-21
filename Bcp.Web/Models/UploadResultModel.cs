namespace Bcp.Web.Models;

public sealed class UploadResultModel
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    private UploadResultModel() { }

    public static UploadResultModel Success() =>
        new()
        {
            IsSuccess = true
        };

    public static UploadResultModel Failure(string errorMessage) =>
        new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
        };
}
