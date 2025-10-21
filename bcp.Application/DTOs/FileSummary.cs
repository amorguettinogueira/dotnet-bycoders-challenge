namespace Bcp.Application.DTOs;

public class FileSummary
{
    public IEnumerable<StoreAggregation> Stores { get; set; } = [];
    public IEnumerable<string> Error { get; set; } = [];
}
