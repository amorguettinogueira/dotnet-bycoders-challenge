using Bcp.Domain.Enums;
using Xunit;

namespace Bcp.Domain.Tests;

public class EnumsTests
{
    [Fact]
    public void NotificationStatus_Has_ExpectedValues()
    {
        Assert.Equal(1, (int)NotificationStatus.Pending);
        Assert.Equal(2, (int)NotificationStatus.Processing);
        Assert.Equal(3, (int)NotificationStatus.Completed);
    }

    [Fact]
    public void TransactionNature_Has_ExpectedValues()
    {
        Assert.Equal(1, (int)TransactionNature.Income);
        Assert.Equal(-1, (int)TransactionNature.Expense);
    }
}
