using Bcp.Domain.Enums;

namespace Bcp.Domain.Models;

/// <summary>
/// Represents a specific type of transaction, including its description and financial nature.
/// </summary>
public class TransactionType
{
    /// <summary>
    /// Primary key for the transaction type.
    /// </summary>
    public int TransactionTypeId { get; set; }

    /// <summary>
    /// Human-readable description of the transaction type (e.g., "Debit", "Boleto", "Financing").
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Indicates whether the transaction type is an income or expense.
    /// </summary>
    public TransactionNature Nature { get; set; }
}