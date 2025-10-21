using Bcp.Domain.Enums;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Persistence;
using Bcp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Infrastructure.Tests;

public class TransactionFileServiceTests
{
    private static AppDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task GetFileSummariesAsync_Returns_Id_And_Name()
    {
        await using var db = NewDb();
        _ = db.Files.Add(new Bcp.Domain.Models.File { FileName = "a.txt" });
        _ = await db.SaveChangesAsync();

        var sut = new TransactionFileService(db);
        var result = await sut.GetFileSummariesAsync();

        _ = Assert.Single(result);
        Assert.True(result[0].FileId > 0);
        Assert.Equal("a.txt", result[0].FileName);
    }

    [Fact]
    public async Task GetStoreAggregationsAsync_Computes_Balances_And_Collects_Errors()
    {
        await using var db = NewDb();
        var store = new Store { StoreName = "S", OwnerName = "O" };
        var typeIncome = new TransactionType { Description = "Credit", Nature = TransactionNature.Income };
        var typeExpense = new TransactionType { Description = "Boleto", Nature = TransactionNature.Expense };
        var file = new Bcp.Domain.Models.File { FileName = "a.txt" };
        _ = db.Stores.Add(store);
        db.Set<TransactionType>().AddRange(typeIncome, typeExpense);
        _ = db.Files.Add(file);
        _ = await db.SaveChangesAsync();

        db.Transactions.AddRange(
            new Transaction { StoreId = store.StoreId, TransactionTypeId = typeIncome.TransactionTypeId, TransactionAmount = 100, FileId = file.FileId },
            new Transaction { StoreId = store.StoreId, TransactionTypeId = typeExpense.TransactionTypeId, TransactionAmount = 30, FileId = file.FileId }
        );
        _ = db.FileError.Add(new FileError { FileId = file.FileId, Error = "err" });
        _ = await db.SaveChangesAsync();

        var sut = new TransactionFileService(db);
        var result = await sut.GetStoreAggregationsAsync(file.FileId);

        _ = Assert.Single(result.Stores);
        var sAgg = result.Stores.First();
        Assert.Equal(store.StoreId, sAgg.StoreId);
        Assert.Equal("S", sAgg.StoreName);
        Assert.Equal(70, sAgg.Balance); // 100 - 30

        _ = Assert.Single(result.Error);
        Assert.Equal("err", result.Error.First());
    }
}