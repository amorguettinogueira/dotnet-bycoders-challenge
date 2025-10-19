namespace bcp.Core.Models;

/// <summary>
/// Represents a transaction beneficiary, identified by CPF and card number.
/// </summary>
public class Beneficiary
{
    /// <summary>
    /// Primary key for the beneficiary record.
    /// </summary>
    public int BeneficiaryId { get; set; }

    /// <summary>
    /// CPF (Cadastro de Pessoas Físicas) of the beneficiary.
    /// Used for personal identification in Brazil.
    /// </summary>
    public required string Cpf { get; set; }

    /// <summary>
    /// Card number associated with the beneficiary.
    /// Use an empty string ("") to represent missing cards.
    /// </summary>
    public required string Card { get; set; }
}
