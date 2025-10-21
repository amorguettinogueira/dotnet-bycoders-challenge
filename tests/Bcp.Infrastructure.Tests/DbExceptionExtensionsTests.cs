using Bcp.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Infrastructure.Tests;

public class DbExceptionExtensionsTests
{
    [Fact]
    public void IsUniqueConstraintViolation_ReturnsTrue_ForKnownMessages()
    {
        var inner1 = new Exception("UNIQUE constraint failed: table.idx");
        var inner2 = new Exception("duplicate key value violates unique constraint");

        var ex1 = new DbUpdateException("msg", inner1);
        var ex2 = new DbUpdateException("msg", inner2);

        Assert.True(ex1.IsUniqueConstraintViolation());
        Assert.True(ex2.IsUniqueConstraintViolation());
    }

    [Fact]
    public void IsUniqueConstraintViolation_ReturnsFalse_OtherMessages()
    {
        var ex = new DbUpdateException("msg", new Exception("something else"));
        Assert.False(ex.IsUniqueConstraintViolation());
    }
}
