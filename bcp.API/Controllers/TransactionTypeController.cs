using bcp.Application.Interfaces;
using bcp.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace bcp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionTypesController : ControllerBase
{
    private readonly ITransactionTypeService _service;

    public TransactionTypesController(ITransactionTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransactionType>>> Get(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        return Ok(result);
    }
}
