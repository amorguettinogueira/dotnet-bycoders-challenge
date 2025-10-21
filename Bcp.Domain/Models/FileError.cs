namespace Bcp.Domain.Models;

/// <summary>
/// Represents a unique file uploaded.
/// </summary>
public class FileError
{
    /// <summary>
    /// Primary key for the transaction.
    /// </summary>
    public int ErrorId { get; set; }

    /// <summary>
    /// Foreign key referencing the file at which this error was raised.
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// Line number at which this error was raised.
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Error raised.
    /// </summary>
    public required string Error { get; set; }

    /// <summary>
    /// Navigation property to the associated File.
    /// </summary>
    public File File { get; set; } = null!;
}