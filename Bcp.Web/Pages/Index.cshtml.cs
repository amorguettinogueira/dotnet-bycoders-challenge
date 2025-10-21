using Bcp.Application.DTOs;
using Bcp.Web.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bcp.Web.Pages;

public class IndexModel(ITransactionFileApi fileApi,
                        IFileUploadService fileUploadService) : PageModel
{
    public List<Application.DTOs.File> Files { get; set; } = [];
    public FileSummary AggregatedData { get; set; } = new();
    public int? SelectedFileId { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync() =>
        Files = await fileApi.GetFilesAsync();

    public async Task<IActionResult> OnGetSelectAsync(int id)
    {
        Files = await fileApi.GetFilesAsync();
        AggregatedData = await fileApi.GetAggregatedDataAsync(id);
        SelectedFileId = id;
        return Page();
    }

    // Drill-down: return transactions for a given file and store as JSON
    public async Task<IActionResult> OnGetTransactionsAsync(int fileId, int storeId)
    {
        var items = await fileApi.GetTransactionsAsync(fileId, storeId);
        return new JsonResult(items);
    }

    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
    {
        if ((files?.Count ?? 0) == 0)
        {
            StatusMessage = "No files were selected for upload.";
            return RedirectToPage();
        }

        var successCount = 0;
        var errors = new List<string>();

        foreach (var file in files!)
        {
            var result = await fileUploadService.UploadFileAsync(file);
            if (result.IsSuccess)
            {
                successCount++;
            }
            else
            {
                errors.Add($"{file.FileName}: {result.ErrorMessage}");
            }
        }

        StatusMessage = successCount > 0
            ? $"{successCount} file(s) uploaded successfully. {(errors.Count != 0 ? string.Join(". ", errors) : "")}"
            : string.Join(". ", errors);

        return RedirectToPage();
    }
}