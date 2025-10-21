using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bcp.Application.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_Returns_ServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplication();

        Assert.Same(services, result);
    }
}