using Bcp.API.Controllers;
using Bcp.Application.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bcp.API.Tests;

public class FilesControllerTests
{
    [Fact]
    public async Task GetFiles_Returns_Ok_With_List()
    {
        var svc = new Mock<ITransactionFileService>();
        var notif = new Mock<IFileNotificationService>();
        _ = svc.Setup(s => s.GetFileSummariesAsync()).ReturnsAsync([new() { FileId = 1, FileName = "a.txt" }]);

        var controller = new FilesController(svc.Object, notif.Object);

        var result = await controller.GetFiles();

        _ = Assert.IsType<OkObjectResult>(result.Result);
        var ok = (OkObjectResult)result.Result!;
        _ = Assert.IsAssignableFrom<List<Bcp.Application.DTOs.File>>(ok.Value);
    }

    [Fact]
    public async Task UploadFile_Returns_BadRequest_When_File_Invalid()
    {
        var svc = new Mock<ITransactionFileService>();
        var notif = new Mock<IFileNotificationService>();
        var controller = new FilesController(svc.Object, notif.Object);

        var result1 = await controller.UploadFile(null!);
        _ = Assert.IsType<BadRequestObjectResult>(result1);

        var formFile = new FormFile(new MemoryStream([]), 0, 0, "file", "bad.bin");
        var result2 = await controller.UploadFile(formFile);
        _ = Assert.IsType<BadRequestObjectResult>(result2);
    }

    [Fact]
    public async Task UploadFile_Returns_Ok_And_Notifies()
    {
        var svc = new Mock<ITransactionFileService>();
        var notif = new Mock<IFileNotificationService>();
        var controller = new FilesController(svc.Object, notif.Object);

        var content = new byte[] { 1, 2, 3 };
        await using var stream = new MemoryStream(content);
        var formFile = new FormFile(stream, 0, content.Length, "file", "good.txt");

        var result = await controller.UploadFile(formFile);

        _ = Assert.IsType<OkObjectResult>(result);
        notif.Verify(n => n.NotifyAsync("good.txt"), Times.Once);
    }
}