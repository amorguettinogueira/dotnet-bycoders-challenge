namespace bcp.Core.Models;

/// <summary>
/// Represents a unique file identified by its size and MD5 hash.
/// </summary>
public class File
{
    /// <summary>
    /// Primary key for the file.
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MD5 hash of the file contents (used for deduplication).
    /// </summary>
    public required string FileHash { get; set; }

    /// <summary>
    /// Collection of all names under which this file was uploaded.
    /// </summary>
    public ICollection<FileName> FileNames { get; set; } = [];

    /// <summary>
    /// Collection of all transactions uploaded in this file.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];
}