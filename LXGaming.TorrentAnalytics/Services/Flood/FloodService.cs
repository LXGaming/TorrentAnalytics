using LXGaming.Configuration.Generic;
using LXGaming.Hosting;
using LXGaming.TorrentAnalytics.Configuration;
using LXGaming.TorrentAnalytics.Services.Flood.Jobs;
using LXGaming.TorrentAnalytics.Services.Torrent.Client;
using LXGaming.TorrentAnalytics.Services.Torrent.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.TorrentAnalytics.Services.Flood;

[Service(ServiceLifetime.Singleton, typeof(ITorrentClientProvider))]
public class FloodService(
    IConfiguration<Config> configuration,
    ILogger<FloodService> logger,
    ISchedulerFactory schedulerFactory,
    IServiceProvider serviceProvider) : IHostedService, ITorrentClientProvider {

    public bool TorrentMetrics => configuration.Value?.FloodCategory.TorrentMetrics == true;

    public bool TrackerMetrics => configuration.Value?.FloodCategory.TrackerMetrics == true;

    public TorrentClientType Type => TorrentClientType.Flood;

    public async Task StartAsync(CancellationToken cancellationToken) {
        var category = configuration.Value?.FloodCategory;
        if (category == null) {
            throw new InvalidOperationException("FloodCategory is unavailable");
        }

        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        if (!string.IsNullOrEmpty(category.Schedule)) {
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

    public ITorrentClient CreateClient(TorrentClientOptions options) {
        return new FloodTorrentClient(options, serviceProvider);
    }
}