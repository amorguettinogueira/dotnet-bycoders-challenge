using bcp.Application.DTOs;
using bcp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bcp.API.Controllers;

/// <summary>
/// File navigation endpoints.
/// </summary>
[ApiController]
[Route("api/files")]
public class FilesController(ITransactionFileService service) : ControllerBase
{
    /// <summary>
    /// Gets a list of all file names previouly imported.
    /// </summary>
    /// <returns>List of file names previouly imported.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FileSummary>>> GetFiles()
    {
        var files = await service.GetFileSummariesAsync();
        return Ok(files);
    }

    /// <summary>
    /// Aggregates all operations within a file by store.
    /// </summary>
    /// <param name="id">File Id.</param>
    /// <returns>List of Store Names and Balance of transaction within a file.</returns>
    [HttpGet("{id}/summary")]
    public async Task<ActionResult<List<StoreAggregation>>> GetFileSummary(int id)
    {
        var summary = await service.GetStoreAggregationsAsync(id);
        return Ok(summary);
    }
}
