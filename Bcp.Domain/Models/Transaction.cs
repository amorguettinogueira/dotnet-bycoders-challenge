namespace Bcp.Domain.Models;

/// <summary>
/// Represents a financial transaction imported from a file.
/// Includes details about the beneficiary, store, and timing.
/// </summary>
public class Transaction
{
    /// <summary>
    /// Primary key for the transaction.
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// Foreign key referencing the file from which this transaction was imported.
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// Foreign key referencing the type of transaction (e.g., Debit, Boleto...).
    /// </summary>
    public int TransactionTypeId { get; set; }

    /// <summary>
    /// Date when the transaction occurred.
    /// </summary>
    public DateOnly DateOfOccurrence { get; set; }

    /// <summary>
    /// Monetary amount involved in the transaction.
    /// </summary>
    public decimal TransactionAmount { get; set; }

    /// <summary>
    /// Identifier of the beneficiary involved in the transaction.
    /// </summary>
    public int BeneficiaryId { get; set; }

    /// <summary>
    /// Time of occurrence in UTC-3 timezone.
    /// Timezone adjustment can be handled during mapping or display.
    /// </summary>
    public TimeSpan TimeOfOccurrence { get; set; }

    /// <summary>
    /// Foreign key referencing the store where the transaction occurred.
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Navigation property to the associated File.
    /// </summary>
    public File File { get; set; } = null!;

    /// <summary>
    /// Navigation property to the associated Beneficiary.
    /// </summary>
    public Beneficiary Beneficiary { get; set; } = null!;

    /// <summary>
    /// Navigation property to the associated Store.
    /// </summary>
    public Store Store { get; set; } = null!;

    /// <summary>
    /// Navigation property to the associated TransactionType.
    /// </summary>
    public TransactionType TransactionType { get; set; } = null!;
}