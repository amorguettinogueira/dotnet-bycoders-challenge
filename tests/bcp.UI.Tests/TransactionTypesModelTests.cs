using bcp.Application.DTOs;
using bcp.Core.Models;
using bcp.UI.Pages;
using bcp.UI.Services;
using Moq;

namespace bcp.UI.Tests;

public class TransactionTypesModelTests
{
    private const string File1Description = nameof(File1Description);
    private const string File2Description = nameof(File2Description);

    [Fact]
    public async Task OnGetAsync_PopulatesTransactionTypes_WhenApiReturnsData()
    {
        // Arrange
        var mockApi = new Mock<ITransactionFileApi>();
        _ = mockApi.Setup(api => api.GetFilesAsync())
               .ReturnsAsync(
               [
                   new FileSummary { FileId = 1, FileName = File1Description },
                   new FileSummary { FileId = 2, FileName = File2Description }
               ]);
        var pageModel = new IndexModel(mockApi.Object);

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
        var mockApi = new Mock<ITransactionFileApi>();
        _ = mockApi.Setup(api => api.GetFilesAsync())
               .ReturnsAsync(Array.Empty<FileSummary>().ToList());

        var pageModel = new IndexModel(mockApi.Object);

        // Act
        await pageModel.OnGetAsync();

        // Assert
        Assert.Empty(pageModel.Files);
    }
}