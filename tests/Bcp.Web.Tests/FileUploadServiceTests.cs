using Bcp.Web.Configuration;
using Bcp.Web.Contracts;
using Bcp.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Refit;

namespace Bcp.Web.Tests;

public class FileUploadServiceTests
{
    private static IOptions<FileUploadOptions> MakeOptions(long maxSize = 1024, params string[] exts) =>
        Options.Create(new FileUploadOptions
        {
            MaxFileSize = maxSize,
            AllowedExtensions = exts.Length == 0 ? [".txt"] : [.. exts]
        });

    private static FormFile NewFormFile(string name, string fileName, byte[] content)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, name, fileName);
    }

    [Fact]
    public async Task UploadFileAsync_Returns_Error_When_Empty()
    {
        var api = new Mock<ITransactionFileApi>();
        var svc = new FileUploadService(api.Object, MakeOptions());
        var form = NewFormFile("file", "a.txt", []);

        var result = await svc.UploadFileAsync(form);

        Assert.False(result.IsSuccess);
        Assert.Contains("empty", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UploadFileAsync_Returns_Error_When_TooLarge()
    {
        var api = new Mock<ITransactionFileApi>();
        var svc = new FileUploadService(api.Object, MakeOptions(maxSize: 1));
        var form = NewFormFile("file", "a.txt", [1, 2]);

        var result = await svc.UploadFileAsync(form);

        Assert.False(result.IsSuccess);
        Assert.Contains("exceeds", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UploadFileAsync_Returns_Error_When_InvalidExtension()
    {
        var api = new Mock<ITransactionFileApi>();
        var svc = new FileUploadService(api.Object, MakeOptions(maxSize: 10, exts: [".txt"]));
        var form = NewFormFile("file", "a.bin", [1]);

        var result = await svc.UploadFileAsync(form);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid file type", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UploadFileAsync_Success_When_Api_Returns_2xx()
    {
        var api = new Mock<ITransactionFileApi>();
        _ = api.Setup(a => a.UploadFileAsync(It.IsAny<StreamPart>()))
           .ReturnsAsync(new ApiResponse<string>(new HttpResponseMessage(System.Net.HttpStatusCode.OK), "ok", null!));
        var svc = new FileUploadService(api.Object, MakeOptions(maxSize: 10));
        var form = NewFormFile("file", "a.txt", [1]);

        var result = await svc.UploadFileAsync(form);

        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFileAsync_Failure_When_Api_Returns_NonSuccess()
    {
        var api = new Mock<ITransactionFileApi>();
        var badResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
        {
            Content = new StringContent("bad")
        };
        var apiEx = await ApiException.Create(new HttpRequestMessage(HttpMethod.Post, "http://test"), HttpMethod.Post, badResponse, new RefitSettings());
        _ = api.Setup(a => a.UploadFileAsync(It.IsAny<StreamPart>()))
           .ReturnsAsync(new ApiResponse<string>(badResponse, apiEx.Content, null!));
        var svc = new FileUploadService(api.Object, MakeOptions(maxSize: 10));
        var form = NewFormFile("file", "a.txt", [1]);

        var result = await svc.UploadFileAsync(form);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFileAsync_Returns_ApiError_When_Api_Throws()
    {
        var api = new Mock<ITransactionFileApi>();
        var badResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        var apiEx = await ApiException.Create(new HttpRequestMessage(HttpMethod.Post, "http://test"), HttpMethod.Post, badResponse, new RefitSettings());
        _ = api.Setup(a => a.UploadFileAsync(It.IsAny<StreamPart>())).ThrowsAsync(apiEx);
        var svc = new FileUploadService(api.Object, MakeOptions(maxSize: 10));
        var form = NewFormFile("file", "a.txt", [1]);

        var result = await svc.UploadFileAsync(form);

        Assert.False(result.IsSuccess);
        Assert.Contains("API Error", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}