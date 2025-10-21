using Bcp.Domain.Enums;
using Bcp.Domain.Models;
using Xunit;

namespace Bcp.Domain.Tests;

public class ModelsTests
{
    [Fact]
    public void Store_CanBeInitialized()
    {
        var s = new Store { StoreId = 5, StoreName = "Shop", OwnerName = "Alice" };
        Assert.Equal(5, s.StoreId);
        Assert.Equal("Shop", s.StoreName);
        Assert.Equal("Alice", s.OwnerName);
    }

    [Fact]
    public void File_CanBeInitialized_With_Default_Collections()
    {
        var f = new Models.File { FileId = 2, FileName = "data.txt" };
        Assert.Equal(2, f.FileId);
        Assert.Equal("data.txt", f.FileName);
        Assert.NotNull(f.Transactions);
        Assert.NotNull(f.Error);
        Assert.Empty(f.Transactions);
        Assert.Empty(f.Error);
    }

    [Fact]
    public void FileError_CanBeInitialized()
    {
        var e = new FileError { ErrorId = 9, FileId = 2, LineNumber = 10, Error = "msg" };
        Assert.Equal(9, e.ErrorId);
        Assert.Equal(2, e.FileId);
        Assert.Equal(10, e.LineNumber);
        Assert.Equal("msg", e.Error);
    }

    [Fact]
    public void FileNotification_CanBeInitialized()
    {
        var n = new FileNotification
        {
            FileNotificationId = 7,
            FileName = "a.txt",
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Status = NotificationStatus.Pending
        };

        Assert.Equal(7, n.FileNotificationId);
        Assert.Equal("a.txt", n.FileName);
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), n.CreatedAt);
        Assert.Equal(NotificationStatus.Pending, n.Status);
    }

    [Fact]
    public void Transaction_CanBeInitialized_And_Linked()
    {
        var store = new Store { StoreId = 1, StoreName = "S", OwnerName = "O" };
        var file = new Models.File { FileId = 3, FileName = "f.txt" };
        var type = new TransactionType { TransactionTypeId = 4, Description = "Debit", Nature = TransactionNature.Income };
        var ben = new Beneficiary { BeneficiaryId = 8, Cpf = "12345678901", Card = "123456789012" };

        var t = new Transaction
        {
            TransactionId = 11,
            FileId = file.FileId,
            TransactionTypeId = type.TransactionTypeId,
            DateOfOccurrence = new DateOnly(2024, 1, 1),
            TransactionAmount = 10.5m,
            BeneficiaryId = ben.BeneficiaryId,
            TimeOfOccurrence = new TimeSpan(10, 0, 0),
            StoreId = store.StoreId,
            File = file,
            Beneficiary = ben,
            Store = store,
            TransactionType = type
        };

        Assert.Equal(11, t.TransactionId);
        Assert.Equal(new DateOnly(2024, 1, 1), t.DateOfOccurrence);
        Assert.Equal(10.5m, t.TransactionAmount);
        Assert.Equal(new TimeSpan(10, 0, 0), t.TimeOfOccurrence);
        Assert.Same(file, t.File);
        Assert.Same(ben, t.Beneficiary);
        Assert.Same(store, t.Store);
        Assert.Same(type, t.TransactionType);
    }
}