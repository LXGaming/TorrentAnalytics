using LXGaming.Configuration.Generic;
using LXGaming.TorrentAnalytics.Configuration;
using LXGaming.TorrentAnalytics.Services.Torrent.Client;
using LXGaming.TorrentAnalytics.Services.Torrent.Models;
using LXGaming.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.TorrentAnalytics.Services.Torrent;

[Service(ServiceLifetime.Singleton)]
public class TorrentService(
    IConfiguration<Config> configuration,
    ILogger<TorrentService> logger,
    IServiceProvider serviceProvider) : IHostedLifecycleService, IDisposable {

    private readonly HashSet<ITorrentClient> _clients = [];
    private bool _disposed;

    public Task StartingAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StartedAsync(CancellationToken cancellationToken) {
        var clientProviders = serviceProvider.GetServices<ITorrentClientProvider>()
            .ToDictionary(provider => provider.Type, provider => provider);
        logger.LogInformation("Discovered {Count} torrent client provider(s)", clientProviders.Count);

        var category = configuration.Value?.TorrentCategory;
        if (category == null) {
            logger.LogWarning("TorrentCategory is unavailable");
            return Task.CompletedTask;
        }

        foreach (var options in category.Clients) {
            if (options.Type == TorrentClientType.None) {
                logger.LogWarning("Type has not been configured for torrent client");
                continue;
            }

            if (!clientProviders.TryGetValue(options.Type, out var clientProvider)) {
                logger.LogWarning("{Type} torrent client provider is unavailable", options.Type);
                continue;
            }

            if (!options.Enabled) {
                logger.LogWarning("{Client} torrent client is not enabled", options);
                continue;
            }

            ITorrentClient client;
            try {
                client = clientProvider.CreateClient(options);
            } catch (Exception ex) {
                logger.LogError(ex, "Encountered an error while creating {Client} torrent client", options);
                continue;
            }

            if (_clients.Add(client)) {
                logger.LogInformation("{Client} torrent client registered", options);
            } else {
                client.Dispose();
                logger.LogWarning("{Client} torrent client is already registered", options);
            }
        }

        if (_clients.Count == 0) {
            logger.LogWarning("No torrent clients configured");
        }

        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public ITorrentClient? GetClient(Type clientType) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return GetClients(clientType).FirstOrDefault();
    }

    public IEnumerable<ITorrentClient> GetClients(Type clientType) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return _clients.Where(clientType.IsInstanceOfType);
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
            foreach (var client in _clients) {
                client.Dispose();
            }
        }
    }
}