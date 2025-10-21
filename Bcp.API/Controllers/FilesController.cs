using Bcp.Application.Contracts;
using Bcp.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Bcp.API.Controllers;

/// <summary>
/// Endpoints for managing CNAB files: list uploaded files, view store aggregations,
/// and upload new files for background processing. Uploads are written to a shared
/// volume and a notification is queued so the worker can pick them up.
/// </summary>
[ApiController]
[Route("api/files")]
public class FilesController(ITransactionFileService service,
                             IFileNotificationService notificationService) : ControllerBase
{
    private readonly string _uploadPath = "/app/uploads";

    /// <summary>
    /// Lists all files that have been uploaded.
    /// </summary>
    /// <remarks>
    /// Returns the file id and the original file name for each upload.
    /// </remarks>
    /// <response code="200">A list of uploaded files.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<Bcp.Application.DTOs.File>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Bcp.Application.DTOs.File>>> GetFiles()
    {
        var files = await service.GetFileSummariesAsync();
        return Ok(files);
    }

    /// <summary>
    /// Gets store-level aggregation for a specific file.
    /// </summary>
    /// <param name="id">The file identifier.</param>
    /// <remarks>
    /// Returns balances per store for the transactions contained in the specified file,
    /// as well as any parse errors associated with it.
    /// </remarks>
    /// <response code="200">Aggregated balances and errors for the file.</response>
    [HttpGet("{id}/summary")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(FileSummary), StatusCodes.Status200OK)]
    public async Task<ActionResult<FileSummary>> GetFileSummary(int id)
    {
        var summary = await service.GetStoreAggregationsAsync(id);
        return Ok(summary);
    }

    /// <summary>
    /// Gets transactions for a given file and store, sorted by date and time.
    /// </summary>
    /// <param name="fileId">File identifier.</param>
    /// <param name="storeId">Store identifier.</param>
    /// <remarks>
    /// Returns a lightweight list with transaction type description, date, time and signed value.
    /// </remarks>
    /// <response code="200">The transactions for the file and store.</response>
    [HttpGet("{fileId}/stores/{storeId}/transactions")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<TransactionItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransactionItem>>> GetTransactions(int fileId, int storeId)
    {
        var items = await service.GetTransactionsAsync(fileId, storeId);
        return Ok(items);
    }

    /// <summary>
    /// Uploads a CNAB `.txt` file for processing.
    /// </summary>
    /// <param name="file">The file to upload (form field name: <c>file</c>).</param>
    /// <remarks>
    /// Only <c>.txt</c> files are accepted. The file is saved to <c>/app/uploads</c> and a
    /// processing notification is queued for the background worker. When processing
    /// completes, a real-time notification is broadcast via SignalR to connected clients.
    /// </remarks>
    /// <response code="200">The file was accepted for processing.</response>
    /// <response code="400">The file is missing or has an invalid type.</response>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        if (!Path.GetExtension(file.FileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid file type. Only .txt files are allowed.");
        }

        _ = Directory.CreateDirectory(_uploadPath);

        var fileName = Path.GetFileName(file.FileName);
        var filePath = $"{_uploadPath}/{fileName}"; // Use forward slashes for Linux containers

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        // Notify with just the file name; worker will resolve full path within its container
        await notificationService.NotifyAsync(fileName);
        return Ok(new { message = "File uploaded successfully" });
    }
}