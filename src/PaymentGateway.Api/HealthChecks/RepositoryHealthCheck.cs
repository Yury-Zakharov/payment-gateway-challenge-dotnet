using Microsoft.Extensions.Diagnostics.HealthChecks;

using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.HealthChecks;

public sealed class RepositoryHealthCheck(IPaymentsRepository repository) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _ = await repository.GetAsync(Guid.Empty, cancellationToken);
            return HealthCheckResult.Healthy("Repository is healthy and responsive");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Repository failed health check", ex);
        }
    }
}
