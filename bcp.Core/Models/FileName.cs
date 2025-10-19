namespace bcp.Core.Models;

/// <summary>
/// Represents a name under which a file was uploaded.
/// A single file can have multiple names associated with it.
/// </summary>
public class FileName
{
    /// <summary>
    /// Primary key for the FileName record.
    /// </summary>
    public int FileNameId { get; set; }

    /// <summary>
    /// Foreign key referencing the associated File.
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// The name under which the file was uploaded.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Navigation property to the associated File.
    /// Populated by EF Core; null suppression is safe.
    /// </summary>
    public File File { get; set; } = null!;
}
