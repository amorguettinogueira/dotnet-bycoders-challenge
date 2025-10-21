using Bcp.Application.DTOs;
using Bcp.Web.Contracts;
using Bcp.Web.Models;
using Bcp.Web.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Bcp.Web.Tests;

public class IndexModelTests
{
    [Fact]
    public async Task OnGetSelectAsync_Loads_Files_And_Summary()
    {
        var fileApi = new Mock<ITransactionFileApi>();
        _ = fileApi.Setup(a => a.GetFilesAsync()).ReturnsAsync([new() { FileId = 1, FileName = "a" }]);
        _ = fileApi.Setup(a => a.GetAggregatedDataAsync(1)).ReturnsAsync(new FileSummary { Stores = [new StoreAggregation { StoreId = 10, StoreName = "s", Balance = 2m }] });
        var upload = new Mock<IFileUploadService>();

        var page = new IndexModel(fileApi.Object, upload.Object);
        var result = await page.OnGetSelectAsync(1);

        _ = Assert.IsType<Microsoft.AspNetCore.Mvc.RazorPages.PageResult>(result);
        _ = Assert.Single(page.Files);
        Assert.Equal(1, page.SelectedFileId);
        _ = Assert.Single(page.AggregatedData.Stores);
    }

    [Fact]
    public async Task OnPostUploadAsync_Handles_NoFiles()
    {
        var fileApi = new Mock<ITransactionFileApi>();
        var upload = new Mock<IFileUploadService>();
        var page = new IndexModel(fileApi.Object, upload.Object);

        var result = await page.OnPostUploadAsync([]);

        _ = Assert.IsType<RedirectToPageResult>(result);
        Assert.Contains("No files were selected", page.StatusMessage);
    }

    [Fact]
    public async Task OnPostUploadAsync_Accumulates_Success_And_Errors()
    {
        var fileApi = new Mock<ITransactionFileApi>();
        var upload = new Mock<IFileUploadService>();
        var toggle = false;
        _ = upload.Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>()))
              .ReturnsAsync(() =>
              {
                  toggle = !toggle;
                  return toggle ? UploadResultModel.Success() : UploadResultModel.Failure("bad");
              });
        var page = new IndexModel(fileApi.Object, upload.Object);

        var files = new List<IFormFile>
        {
            new FormFile(new MemoryStream([1]), 0, 1, "f1", "f1.txt"),
            new FormFile(new MemoryStream([1]), 0, 1, "f2", "f2.txt"),
            new FormFile(new MemoryStream([1]), 0, 1, "f3", "f3.txt"),
        };

        var result = await page.OnPostUploadAsync(files);

        _ = Assert.IsType<RedirectToPageResult>(result);
        Assert.Contains("2 file(s) uploaded successfully", page.StatusMessage);
        // Only failed files are listed; here f2 fails
        Assert.Contains("f2.txt", page.StatusMessage);
        Assert.DoesNotContain("f1.txt", page.StatusMessage);
        Assert.DoesNotContain("f3.txt", page.StatusMessage);
    }
}