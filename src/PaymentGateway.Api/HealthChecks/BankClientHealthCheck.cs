using Microsoft.Extensions.Diagnostics.HealthChecks;
using PaymentGateway.Api.Clients;

namespace PaymentGateway.Api.HealthChecks;

public sealed class BankClientHealthCheck(IBankClient bankClient) : IHealthCheck
{
    private readonly IBankClient _bankClient = bankClient;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // As IBankClient implements only POST, which is not idempotent, I left implementation like this. Needs actual probe in real system.
        return Task.FromResult(HealthCheckResult.Healthy("Bank client is registered and ready"));
    }
}