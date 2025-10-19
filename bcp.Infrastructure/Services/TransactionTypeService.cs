using bcp.Application.Interfaces;
using bcp.Core.Models;
using bcp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace bcp.Infrastructure.Repositories;

public class TransactionTypeService(AppDbContext context) : ITransactionTypeService
{
    public async Task<List<TransactionType>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.TransactionTypes.ToListAsync(cancellationToken);
}
