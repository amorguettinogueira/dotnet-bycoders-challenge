using Bcp.Domain.Models;
using Bcp.Infrastructure.Persistence;
using Bcp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bcp.Infrastructure.Tests;

public class ResolverTests
{
    private static AppDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task BeneficiaryResolver_Returns_Existing()
    {
        await using var db = NewDb();
        _ = db.Beneficiaries.Add(new Beneficiary { Cpf = "1", Card = "2" });
        _ = await db.SaveChangesAsync();

        var logger = new Mock<ILogger<BeneficiaryResolver>>();
        var sut = new BeneficiaryResolver(db, logger.Object);

        var res = await sut.GetOrAddAsync("1", "2", CancellationToken.None);
        Assert.Equal("1", res.Cpf);
        Assert.Equal("2", res.Card);
    }

    [Fact]
    public async Task StoreResolver_Returns_Existing()
    {
        await using var db = NewDb();
        _ = db.Stores.Add(new Store { StoreName = "A", OwnerName = "B" });
        _ = await db.SaveChangesAsync();

        var logger = new Mock<ILogger<BeneficiaryResolver>>();
        var sut = new StoreResolver(db, logger.Object);

        var res = await sut.GetOrAddAsync("A", "B", CancellationToken.None);
        Assert.Equal("A", res.StoreName);
        Assert.Equal("B", res.OwnerName);
    }
}