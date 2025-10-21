using Bcp.Application.Contracts;
using Bcp.Application.DTOs;
using Bcp.Domain.Enums;
using Bcp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Infrastructure.Repositories;

public class TransactionFileService(AppDbContext db) : ITransactionFileService
{
    public async Task<List<Application.DTOs.File>> GetFileSummariesAsync() =>
        await db.Files
            .Select(fn => new Application.DTOs.File
            {
                FileId = fn.FileId,
                FileName = fn.FileName
            })
            .ToListAsync();

    public async Task<FileSummary> GetStoreAggregationsAsync(int fileId) =>
        new FileSummary()
        {
            Stores = await db.Transactions
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
                }).ToArrayAsync(),
            Error = await db.FileError
                .Where(f => f.FileId == fileId)
                .Select(fe => (fe.LineNumber ?? 0) > 0
                    ? $"At line {fe.LineNumber}: {fe.Error}"
                    : fe.Error)
                .ToArrayAsync(),
        };
}
