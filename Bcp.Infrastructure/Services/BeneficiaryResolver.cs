using Bcp.Application.Contracts;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Extensions;
using Bcp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bcp.Infrastructure.Services;

public class BeneficiaryResolver(AppDbContext db, ILogger<BeneficiaryResolver> logger) : IBeneficiaryResolver
{
    public async Task<Beneficiary> GetOrAddAsync(string cpf, string card, CancellationToken cancellationToken)
    {
        var existing = await db.Beneficiaries
            .FirstOrDefaultAsync(b => b.Cpf == cpf && b.Card == card, cancellationToken);

        if (existing != null)
        {
            return existing;
        }

        try
        {
            var newBeneficiary = new Beneficiary { Cpf = cpf, Card = card };
            _ = db.Beneficiaries.Add(newBeneficiary);
            await db.SaveChangesAsync(cancellationToken);
            return newBeneficiary;
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            logger.LogWarning($"Beneficiary CPF {cpf} + Card {card} already existed. Skipping.");
        }

        return await db.Beneficiaries.FirstOrDefaultAsync(b => b.Cpf == cpf && b.Card == card, cancellationToken)
            ?? throw new InvalidOperationException($"Beneficiary CPF {cpf} + Card {card} wasn't found and could not be added.");
    }
}
