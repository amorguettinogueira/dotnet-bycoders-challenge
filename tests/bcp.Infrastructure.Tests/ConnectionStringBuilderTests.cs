using bcp.Infrastructure.Configuration;

namespace bcp.Infrastructure.Tests;

public class ConnectionStringBuilderTests
{
    [Fact]
    public void BuildFromEnvironment_ReturnsCorrectConnectionString_WhenAllVariablesAreSet()
    {
        // Arrange
        Environment.SetEnvironmentVariable("POSTGRES_DB", "testdb");
        Environment.SetEnvironmentVariable("POSTGRES_USER", "testuser");
        Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", "testpass");

        // Act
        var result = ConnectionStringBuilder.BuildFromEnvironment();

        // Assert
        Assert.Equal("Host=db;Database=testdb;Username=testuser;Password=testpass", result);
    }

    [Theory]
    [InlineData("POSTGRES_DB")]
    [InlineData("POSTGRES_USER")]
    [InlineData("POSTGRES_PASSWORD")]
    public void BuildFromEnvironment_ThrowsInvalidDataException_WhenVariableIsMissing(string missingKey)
    {
        // Arrange
        Environment.SetEnvironmentVariable("POSTGRES_DB", "testdb");
        Environment.SetEnvironmentVariable("POSTGRES_USER", "testuser");
        Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", "testpass");

        Environment.SetEnvironmentVariable(missingKey, null); // Simulate missing variable

        // Act
        var ex = Assert.Throws<InvalidDataException>(() => ConnectionStringBuilder.BuildFromEnvironment());

        // Assert
        Assert.Contains($"You are missing an environment variable '{missingKey}'", ex.Message);
    }
}
