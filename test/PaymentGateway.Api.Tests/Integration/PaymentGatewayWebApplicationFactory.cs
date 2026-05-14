using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.Models.Configuration;

namespace PaymentGateway.Api.Tests.Integration;

public sealed class PaymentGatewayWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly BankSimulatorContainer _bankContainer = new();

    public ValueTask InitializeAsync() => _bankContainer.InitializeAsync();

    public ValueTask DisposeAsync() => _bankContainer.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure<BankClientOptions>(options =>
                options.BaseUrl = _bankContainer.BaseUrl);
        });
    }
}