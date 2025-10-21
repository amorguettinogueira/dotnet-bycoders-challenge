using Bcp.Domain.Enums;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Persistence;
using Bcp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Infrastructure.Tests;

public class TransactionFileServiceTransactionsTests
{
    private static AppDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task GetTransactionsAsync_Filters_By_File_And_Store_And_Sorts()
    {
        await using var db = NewDb();
        var typeIncome = new TransactionType { Description = "Credit", Nature = TransactionNature.Income };
        var typeExpense = new TransactionType { Description = "Boleto", Nature = TransactionNature.Expense };
        db.Set<TransactionType>().AddRange(typeIncome, typeExpense);

        var store1 = new Store { StoreName = "S1", OwnerName = "O1" };
        var store2 = new Store { StoreName = "S2", OwnerName = "O2" };
        db.Stores.AddRange(store1, store2);
        var f1 = new Bcp.Domain.Models.File { FileName = "a.txt" };
        var f2 = new Bcp.Domain.Models.File { FileName = "b.txt" };
        db.Files.AddRange(f1, f2);
        _ = await db.SaveChangesAsync();

        // File 1 / Store 1
        db.Transactions.AddRange(
            new Transaction { FileId = f1.FileId, StoreId = store1.StoreId, TransactionTypeId = typeExpense.TransactionTypeId, DateOfOccurrence = new DateOnly(2024, 10, 22), TimeOfOccurrence = new TimeSpan(10, 30, 0), TransactionAmount = 50 },
            new Transaction { FileId = f1.FileId, StoreId = store1.StoreId, TransactionTypeId = typeIncome.TransactionTypeId, DateOfOccurrence = new DateOnly(2024, 10, 22), TimeOfOccurrence = new TimeSpan(9, 15, 0), TransactionAmount = 100 }
        );
        // Other combinations to ensure filtering
        db.Transactions.AddRange(
            new Transaction { FileId = f1.FileId, StoreId = store2.StoreId, TransactionTypeId = typeIncome.TransactionTypeId, DateOfOccurrence = new DateOnly(2024, 10, 21), TimeOfOccurrence = new TimeSpan(8, 0, 0), TransactionAmount = 5 },
            new Transaction { FileId = f2.FileId, StoreId = store1.StoreId, TransactionTypeId = typeIncome.TransactionTypeId, DateOfOccurrence = new DateOnly(2024, 9, 21), TimeOfOccurrence = new TimeSpan(8, 0, 0), TransactionAmount = 5 }
        );
        _ = await db.SaveChangesAsync();

        var sut = new TransactionFileService(db);
        var result = await sut.GetTransactionsAsync(f1.FileId, store1.StoreId);

        Assert.Equal(2, result.Count);
        Assert.Equal("Credit", result[0].TransactionType); // 09:15 first
        Assert.Equal("Boleto", result[1].TransactionType); // 10:30 second
        Assert.Equal(100m, result[0].Value);
        Assert.Equal(-50m, result[1].Value);
    }
}