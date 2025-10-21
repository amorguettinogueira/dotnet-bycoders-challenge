using Bcp.Application.Contracts;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Extensions;
using Bcp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bcp.Infrastructure.Services;

public class StoreResolver(AppDbContext db, ILogger<BeneficiaryResolver> logger) : IStoreResolver
{
    public async Task<Store> GetOrAddAsync(string name, string owner, CancellationToken cancellationToken)
    {
        var existing = await db.Stores
            .FirstOrDefaultAsync(b => b.StoreName == name && b.OwnerName == owner, cancellationToken);

        if (existing != null)
        {
            return existing;
        }

        try
        {
            var newStore = new Store { StoreName = name, OwnerName = owner };
            _ = db.Stores.Add(newStore);
            await db.SaveChangesAsync(cancellationToken);
            return newStore;
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            logger.LogWarning($"Store Name {name} + Owner {owner} already existed. Skipping.");
        }

        return await db.Stores.FirstOrDefaultAsync(b => b.StoreName == name && b.OwnerName == owner, cancellationToken)
        ?? throw new InvalidOperationException($"Store Name  {name}  + Owner  {owner} wasn't found and could not be added.");
    }
}
