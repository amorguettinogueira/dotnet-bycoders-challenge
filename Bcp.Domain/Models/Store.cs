namespace Bcp.Domain.Models;

/// <summary>
/// Represents a store where transactions occur.
/// Each store has a unique name and is owned by a specific person.
/// </summary>
public class Store
{
    /// <summary>
    /// Primary key for the store.
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Name of the store.
    /// </summary>
    public required string StoreName { get; set; }

    /// <summary>
    /// Name of the store owner.
    /// </summary>
    public required string OwnerName { get; set; }
}