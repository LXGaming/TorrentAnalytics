using System.Net;
using LXGaming.Configuration.Generic;
using LXGaming.FloodAnalytics.Configuration;
using LXGaming.FloodAnalytics.Services.Flood.Jobs;
using LXGaming.FloodAnalytics.Services.Flood.Models;
using LXGaming.FloodAnalytics.Services.Web;
using LXGaming.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.FloodAnalytics.Services.Flood;

[Service(ServiceLifetime.Singleton)]
public class FloodService(
    IConfiguration<Config> configuration,
    ILogger<FloodService> logger,
    ISchedulerFactory schedulerFactory,
    WebService webService) : IHostedService, IDisposable {

    private const uint DefaultReconnectDelay = 2;
    private const uint DefaultMaximumReconnectDelay = 300; // 5 Minutes
    private HttpClient? _httpClient;
    private bool _disposed;

    public async Task StartAsync(CancellationToken cancellationToken) {
        var category = configuration.Value?.FloodCategory;
        if (category == null) {
            logger.LogWarning("FloodCategory is unavailable");
            return;
        }

        if (string.IsNullOrEmpty(category.Address)) {
            logger.LogWarning("Flood address has not been configured");
            return;
        }

        var handler = webService.CreateHandler();
        handler.CookieContainer = new CookieContainer();
        handler.UseCookies = true;

        _httpClient = webService.CreateClient(handler);
        _httpClient.BaseAddress = new Uri(category.Address);

        foreach (var pair in category.AdditionalHeaders) {
            if (!_httpClient.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, pair.Value)) {
                logger.LogWarning("Failed to add {Header} header to Flood", pair.Key);
            }
        }

        var reconnectDelay = DefaultReconnectDelay;
        while (true) {
            try {
                var authenticate = await AuthenticateAsync();
                if (!authenticate.Success) {
                    logger.LogWarning("Flood authentication failed");
                    return;
                }

                logger.LogInformation("Connected to Flood as {Username} ({Level})", authenticate.Username, authenticate.Level);
                break;
            } catch (HttpRequestException ex) {
                if (ex is { StatusCode: HttpStatusCode.Unauthorized }) {
                    logger.LogWarning(ex, "Flood authentication failed");
                    return;
                }

                var delay = TimeSpan.FromSeconds(reconnectDelay);
                reconnectDelay = Math.Min(reconnectDelay << 1, DefaultMaximumReconnectDelay);

                logger.LogWarning("Connection failed! Next attempt in {Delay}: {Message}", delay, ex.Message);
                await Task.Delay(delay, cancellationToken);
            }
        }

        if (!string.IsNullOrEmpty(category.Schedule)) {
            var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            await scheduler.ScheduleJob(
                JobBuilder.Create<FloodJob>().WithIdentity(FloodJob.JobKey).Build(),
                TriggerBuilder.Create().WithCronSchedule(category.Schedule).Build(),
                cancellationToken);
        } else {
            logger.LogWarning("Flood schedule has not been configured");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public async Task<T> EnsureAuthenticatedAsync<T>(Func<Task<T>> task) {
        Exception exception;
        try {
            return await task();
        } catch (HttpRequestException ex) {
            if (ex is not { StatusCode: HttpStatusCode.Unauthorized }) {
                throw;
            }

            exception = ex;
        }

        var authenticate = await AuthenticateAsync();
        if (!authenticate.Success) {
            logger.LogWarning("Reconnection failed!");
            throw exception;
        }

        logger.LogInformation("Reconnected to Flood as {Username} ({Level})", authenticate.Username, authenticate.Level);
        return await task();
    }

    public async Task<Authenticate> AuthenticateAsync() {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        var category = configuration.Value?.FloodCategory;
        if (category == null) {
            throw new InvalidOperationException("FloodCategory is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/authenticate");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string?> {
            { "username", category.Username },
            { "password", category.Password }
        });
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await webService.DeserializeAsync<Authenticate>(response);
    }

    public async Task<TorrentListSummary> GetTorrentsAsync() {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/torrents");
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await webService.DeserializeAsync<TorrentListSummary>(response);
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
            _httpClient?.Dispose();
        }
    }
}