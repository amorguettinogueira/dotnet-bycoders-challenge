using Bcp.Application.Contracts;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Contracts;
using Bcp.Infrastructure.Models;

namespace Bcp.Infrastructure.Services;

public class TransactionParser(IBeneficiaryResolver beneficiaryResolver,
                               IStoreResolver storeResolver) : ITransactionParser
{
    public async Task<List<Transaction>> ParseBatchAsync(TransactionRecord[] records, CancellationToken cancellationToken)
    {
        var transactions = new List<Transaction>();

        foreach (TransactionRecord record in records)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var transaction = new Transaction
            {
                TransactionTypeId = record.Type,
                DateOfOccurrence = record.Date,
                TransactionAmount = record.Value,
                TimeOfOccurrence = record.Time,
                Beneficiary = await beneficiaryResolver.GetOrAddAsync(record.CPF, record.Card, cancellationToken),
                Store = await storeResolver.GetOrAddAsync(record.StoreName, record.StoreOwner, cancellationToken),
            };
            transactions.Add(transaction);
        }

        return transactions;
    }
}
