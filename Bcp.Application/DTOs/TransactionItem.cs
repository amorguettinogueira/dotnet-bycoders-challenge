namespace Bcp.Application.DTOs;

/// <summary>
/// Lightweight transaction view for drill-down lists.
/// </summary>
public sealed class TransactionItem
{
    /// <summary>
    /// Transaction type description (e.g., Credit, Boleto, etc.).
    /// </summary>
    public required string TransactionType { get; set; }

    /// <summary>
    /// Date of occurrence.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Time of occurrence (UTC-3 in source file; stored as TimeSpan).
    /// </summary>
    public TimeSpan Time { get; set; }

    /// <summary>
    /// Signed amount: negative for expense types, positive for income types.
    /// </summary>
    public decimal Value { get; set; }
}
