using InfluxDB.Client;
using LXGaming.Configuration.Generic;
using LXGaming.FloodAnalytics.Configuration;
using LXGaming.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LXGaming.FloodAnalytics.Services.InfluxDb;

[Service(ServiceLifetime.Singleton)]
public class InfluxDbService(IConfiguration<Config> configuration) : IHostedService, IDisposable {

    public InfluxDBClient? Client { get; private set; }

    private bool _disposed;

    public Task StartAsync(CancellationToken cancellationToken) {
        var category = configuration.Value?.InfluxDbCategory;
        if (category == null) {
            throw new InvalidOperationException("InfluxDbCategory is unavailable");
        }

        if (string.IsNullOrEmpty(category.Url)) {
            throw new InvalidOperationException("Url has not been configured for InfluxDb");
        }

        if (string.IsNullOrEmpty(category.Token)) {
            throw new InvalidOperationException("Token has not been configured for InfluxDb");
        }

        if (string.IsNullOrEmpty(category.Bucket)) {
            throw new InvalidOperationException("Bucket has not been configured for InfluxDb");
        }

        if (string.IsNullOrEmpty(category.Organization)) {
            throw new InvalidOperationException("Organization has not been configured for InfluxDb");
        }

        Client ??= new InfluxDBClient(new InfluxDBClientOptions(category.Url) {
            Bucket = category.Bucket,
            Org = category.Organization,
            Token = category.Token
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
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
            Client?.Dispose();
        }
    }
}