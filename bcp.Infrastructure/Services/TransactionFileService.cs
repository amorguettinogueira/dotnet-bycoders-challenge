using bcp.Application.DTOs;
using bcp.Application.Interfaces;
using bcp.Core.Enums;
using bcp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace bcp.Infrastructure.Repositories;

public class TransactionFileService(AppDbContext db) : ITransactionFileService
{
    public async Task<List<FileSummary>> GetFileSummariesAsync() =>
        await db.FileNames
            .Select(fn => new FileSummary
            {
                FileId = fn.FileId,
                FileName = fn.Name
            })
            .ToListAsync();

    public async Task<List<StoreAggregation>> GetStoreAggregationsAsync(int fileId) =>
        await db.Transactions
            .Where(t => t.FileId == fileId)
            .Select(t => new
            {
                t.StoreId,
                t.Store.StoreName,
                AdjustedAmount = t.TransactionType.Nature == TransactionNature.Expense
                    ? -t.TransactionAmount
                    : t.TransactionAmount
            })
            .GroupBy(x => new { x.StoreId, x.StoreName })
            .Select(g => new StoreAggregation
            {
                StoreId = g.Key.StoreId,
                StoreName = g.Key.StoreName,
                Balance = g.Sum(x => x.AdjustedAmount)
            })
            .ToListAsync();
}
