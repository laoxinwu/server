using Duende.IdentityServer.ResponseHandling;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bit.Identity.HealthChecks;

public class IdentityServerHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;

    public IdentityServerHealthCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            // Check if IdentityServer services are available
            var discoveryResponseGenerator = scope.ServiceProvider.GetService<IDiscoveryResponseGenerator>();

            if (discoveryResponseGenerator == null)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("IdentityServer services not available"));
            }

            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("IdentityServer check failed", ex));
        }
    }
}
