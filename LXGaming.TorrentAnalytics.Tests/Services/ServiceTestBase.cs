using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace LXGaming.TorrentAnalytics.Tests.Services;

public abstract class ServiceTestBase : IDisposable {

    protected IServiceCollection Services { get; }
    protected IServiceProvider Provider => _serviceProvider.Value;

    private readonly Lazy<ServiceProvider> _serviceProvider;
    private bool _disposed;

    protected ServiceTestBase() {
        Services = new ServiceCollection();
        _serviceProvider = new Lazy<ServiceProvider>(BuildServiceProvider);
    }

    [OneTimeSetUp]
    public async Task StartAsync() {
        foreach (var hostedService in Provider.GetServices<IHostedService>()) {
            var hostedLifecycleService = hostedService as IHostedLifecycleService;

            if (hostedLifecycleService != null) {
                await hostedLifecycleService.StartingAsync(CancellationToken.None);
            }

            await hostedService.StartAsync(CancellationToken.None);

            if (hostedLifecycleService != null) {
                await hostedLifecycleService.StartedAsync(CancellationToken.None);
            }
        }
    }

    [OneTimeTearDown]
    public async Task StopAsync() {
        foreach (var hostedService in Provider.GetServices<IHostedService>()) {
            var hostedLifecycleService = hostedService as IHostedLifecycleService;

            if (hostedLifecycleService != null) {
                await hostedLifecycleService.StoppingAsync(CancellationToken.None);
            }

            await hostedService.StopAsync(CancellationToken.None);

            if (hostedLifecycleService != null) {
                await hostedLifecycleService.StoppedAsync(CancellationToken.None);
            }
        }
    }

    private ServiceProvider BuildServiceProvider() {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return Services.BuildServiceProvider();
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        _disposed = true;

        if (disposing) {
            if (_serviceProvider.IsValueCreated) {
                _serviceProvider.Value.Dispose();
            }
        }
    }
}