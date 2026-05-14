
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace PaymentGateway.Api.Tests.Integration;

public sealed class BankSimulatorContainer : IAsyncLifetime
{
    private readonly IContainer _container;

    public string BaseUrl => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(8080)}";

    public BankSimulatorContainer()
    {
        var impostersPath = Path.Combine(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath, "imposters");
        _container = new ContainerBuilder("bbyars/mountebank:2.8.1")
            .WithPortBinding(8080, true)
            .WithPortBinding(2525, true)   // mountebank management UI (always ready)
            .WithCreateParameterModifier(p =>
            {
                p.HostConfig ??= new();
                p.HostConfig.Binds = [$"{impostersPath}:/imposters:Z"];
            })
            .WithCommand("--configfile", "/imposters/bank_simulator.ejs", "--allowInjection")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(2525)))
            .Build();
    }

    public async ValueTask InitializeAsync() => await _container.StartAsync();
    public ValueTask DisposeAsync() => _container.DisposeAsync();
    
}