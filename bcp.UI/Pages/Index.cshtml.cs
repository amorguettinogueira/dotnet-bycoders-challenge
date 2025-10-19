using bcp.Application.DTOs;
using bcp.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bcp.UI.Pages;

public class IndexModel(ITransactionFileApi service) : PageModel
{
    public List<FileSummary> Files { get; set; } = new();
    public List<StoreAggregation> AggregatedData { get; set; } = new();
    public int? SelectedFileId { get; set; }

    public async Task OnGetAsync() =>
        Files = await service.GetFilesAsync();

    public async Task<IActionResult> OnGetSelectAsync(int id)
    {
        Files = await service.GetFilesAsync();
        AggregatedData = await service.GetAggregatedDataAsync(id);
        SelectedFileId = id;
        return Page();
    }
}
