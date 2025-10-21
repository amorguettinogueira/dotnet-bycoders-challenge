using Bcp.Application.DTOs;
using Xunit;

namespace Bcp.Application.Tests;

public class DtosTests
{
    [Fact]
    public void File_DTO_CanBeInitialized()
    {
        var f = new DTOs.File { FileId = 10, FileName = "abc.txt" };
        Assert.Equal(10, f.FileId);
        Assert.Equal("abc.txt", f.FileName);
    }

    [Fact]
    public void StoreAggregation_DTO_CanBeInitialized()
    {
        var s = new StoreAggregation { StoreId = 2, StoreName = "Market", Balance = 123.45m };
        Assert.Equal(2, s.StoreId);
        Assert.Equal("Market", s.StoreName);
        Assert.Equal(123.45m, s.Balance);
    }

    [Fact]
    public void FileSummary_Default_Collections_Are_Not_Null()
    {
        var fs = new FileSummary();
        Assert.NotNull(fs.Stores);
        Assert.NotNull(fs.Error);
        Assert.Empty(fs.Stores);
        Assert.Empty(fs.Error);
    }

    [Fact]
    public void FileSummary_CanHold_Data()
    {
        var fs = new FileSummary
        {
            Stores = [new StoreAggregation { StoreId = 1, StoreName = "A", Balance = 1m }],
            Error = ["err1", "err2"]
        };

        _ = Assert.Single(fs.Stores);
        Assert.Equal(2, fs.Error.Count());
    }
}