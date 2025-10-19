using bcp.Core.Models;
using bcp.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bcp.UI.Pages;

public class IndexModel(ITransactionTypeApi service) : PageModel
{
    public List<TransactionType> TransactionTypes { get; set; } = new();

    public async Task OnGetAsync() => 
        TransactionTypes = await service.GetAllAsync();
}
