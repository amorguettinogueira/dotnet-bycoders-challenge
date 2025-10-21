using Bcp.Web.Contracts;
using Bcp.Web.Models;
using Bcp.Web.Pages;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Bcp.Web.Tests;

public class TransactionTypesModelTests
{
    private const string File1Description = nameof(File1Description);
    private const string File2Description = nameof(File2Description);

    [Fact]
    public async Task OnGetAsync_PopulatesTransactionTypes_WhenApiReturnsData()
    {
        // Arrange
        var mockFileApi = new Mock<ITransactionFileApi>();
        _ = mockFileApi.Setup(api => api.GetFilesAsync())
               .ReturnsAsync(
               [
                   new Application.DTOs.File { FileId = 1, FileName = File1Description },
                   new Application.DTOs.File { FileId = 2, FileName = File2Description }
               ]);
        var mockUploadService = new Mock<IFileUploadService>();
        _ = mockUploadService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(UploadResultModel.Success());

        var pageModel = new IndexModel(mockFileApi.Object, mockUploadService.Object);

        // Act
        await pageModel.OnGetAsync();

        // Assert
        Assert.Equal(2, pageModel.Files.Count);
        Assert.Equal(File1Description, pageModel.Files[0].FileName);
        Assert.Equal(File2Description, pageModel.Files[1].FileName);
    }

    [Fact]
    public async Task OnGetAsync_SetsEmptyList_WhenApiReturnsNull()
    {
        // Arrange
        var mockFileApi = new Mock<ITransactionFileApi>();
        _ = mockFileApi.Setup(api => api.GetFilesAsync())
               .ReturnsAsync([]);
        var mockUploadService = new Mock<IFileUploadService>();
        _ = mockUploadService.Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(UploadResultModel.Success());

        var pageModel = new IndexModel(mockFileApi.Object, mockUploadService.Object);

        // Act
        await pageModel.OnGetAsync();

        // Assert
        Assert.Empty(pageModel.Files);
    }
}