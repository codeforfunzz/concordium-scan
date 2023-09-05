using Application.Aggregates.Contract.Observability;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tests.Aggregates.Contract.Observability;

public sealed class ContractHealthCheckTests
{
    [Fact]
    public async Task GivenNoFailedJob_WhenCallingHealthCheck_ThenReturnHealthy()
    {
        // Arrange
        var healthCheck = new ContractHealthCheck();
        
        // Act
        var checkHealthAsync = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        
        // Assert
        checkHealthAsync.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task GivenFailedJob_WhenCallingHealthCheck_ThenReturnUnhealthy()
    {
        // Arrange
        const string key = "foo";
        const string value = "value";
        var healthCheck = new ContractHealthCheck();
        healthCheck.AddUnhealthyJobWithMessage(key, value);
        
        // Act
        var checkHealthAsync = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        
        // Assert
        checkHealthAsync.Status.Should().Be(HealthStatus.Degraded);
        checkHealthAsync.Data[key].Should().Be(value);
    }
}