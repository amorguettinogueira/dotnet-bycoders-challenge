using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bcp.UI.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel(ILogger<ErrorModel> logger) : PageModel
{
    public readonly static string HTTP_ERROR_CODE_PREFIX = "HTTP-";

    public string? ErrorMessage { get; set; }

    public int HttpErrorCode
    {
        get
        {
            var errorCode = (ErrorMessage ?? string.Empty).StartsWith(HTTP_ERROR_CODE_PREFIX)
                ? ErrorMessage!.Replace(HTTP_ERROR_CODE_PREFIX, string.Empty)
                : string.Empty;
            return int.TryParse(errorCode, out var value) ? value : 0;
        }
    }

    public void OnGet([FromQuery] string? errorMessage)
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        // fallback if you pass a query param or for non-exception flows
        ErrorMessage = exception?.Message ?? errorMessage ?? "Unexpected error!";

        logger.LogError("Error Message: {ErrorMessage}", ErrorMessage);
    }
}