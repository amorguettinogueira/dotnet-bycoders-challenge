namespace Bcp.Application.DTOs;

public class StoreAggregation
{
    public int StoreId { get; set; }
    public required string StoreName { get; set; }
    public decimal Balance { get; set; }
}
