using Microsoft.EntityFrameworkCore;

namespace Bcp.Infrastructure.Extensions;

public static class DbExceptionExtensions
{
    public static bool IsUniqueConstraintViolation(this DbUpdateException exception) =>
        exception.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true
            || exception.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true;
}