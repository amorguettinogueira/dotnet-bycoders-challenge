namespace Bcp.Domain.Models;

/// <summary>
/// Represents a unique file uploaded.
/// </summary>
public class File
{
    /// <summary>
    /// Primary key for the file.
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// File name under which this file was uploaded.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Collection of all transactions uploaded in this file.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];

    /// <summary>
    /// Collection of all error raised during this file parse.
    /// </summary>
    public ICollection<FileError> Error { get; set; } = [];
}