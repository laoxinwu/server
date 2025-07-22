using Bit.Infrastructure.EntityFramework.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace Bit.Identity.HealthChecks;

public class DatabaseConnectionHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseConnectionHealthCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            // Simple query test
            await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");

            return HealthCheckResult.Healthy();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy("Database connection check was cancelled");
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("Database connection timed out after 5 seconds");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Database connection failed: {ex.Message}", ex);
        }
    }
}
