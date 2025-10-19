namespace bcp.Core.Enums;

/// <summary>
/// Defines the nature of a transaction, indicating whether it represents income or expense.
/// </summary>
public enum TransactionNature
{
    /// <summary>
    /// Represents a transaction that adds funds (e.g., Debit, Credit).
    /// </summary>
    Income = 1,

    /// <summary>
    /// Represents a transaction that subtracts funds (e.g., Boleto, Financing).
    /// </summary>
    Expense = -1
}

