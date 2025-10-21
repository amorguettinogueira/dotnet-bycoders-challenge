namespace Bcp.Web.Configuration;

public class FileUploadOptions
{
    public const string Section = "FileUpload";
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB default
    public List<string> AllowedExtensions { get; set; } = [".txt"];
}