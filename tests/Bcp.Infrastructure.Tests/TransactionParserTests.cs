using Bcp.Application.Contracts;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Models;
using Bcp.Infrastructure.Services;
using Moq;

namespace Bcp.Infrastructure.Tests;

public class TransactionParserTests
{
    [Fact]
    public async Task ParseBatchAsync_Maps_Fields_And_Resolves_Dependencies()
    {
        var ben = new Beneficiary { BeneficiaryId = 1, Cpf = "cpf", Card = "card" };
        var store = new Store { StoreId = 2, StoreName = "s", OwnerName = "o" };

        var b = new Mock<IBeneficiaryResolver>();
        _ = b.Setup(x => x.GetOrAddAsync("cpf", "card", It.IsAny<CancellationToken>())).ReturnsAsync(ben);
        var s = new Mock<IStoreResolver>();
        _ = s.Setup(x => x.GetOrAddAsync("shop", "owner", It.IsAny<CancellationToken>())).ReturnsAsync(store);

        var sut = new TransactionParser(b.Object, s.Object);

        var records = new[]
        {
            new TransactionRecord
            {
                Type = 1,
                Date = new DateOnly(2024,1,1),
                Value = 10,
                CPF = "cpf",
                Card = "card",
                Time = new TimeSpan(1,0,0),
                StoreOwner = "owner",
                StoreName = "shop"
            }
        };

        var trans = await sut.ParseBatchAsync(records, CancellationToken.None);

        _ = Assert.Single(trans);
        var t = trans[0];
        Assert.Equal(1, t.TransactionTypeId);
        Assert.Equal(new DateOnly(2024, 1, 1), t.DateOfOccurrence);
        Assert.Equal(10, t.TransactionAmount);
        Assert.Equal(new TimeSpan(1, 0, 0), t.TimeOfOccurrence);
        Assert.Same(ben, t.Beneficiary);
        Assert.Same(store, t.Store);
    }
}