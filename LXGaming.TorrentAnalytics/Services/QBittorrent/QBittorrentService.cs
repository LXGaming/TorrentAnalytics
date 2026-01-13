using LXGaming.Configuration.Generic;
using LXGaming.TorrentAnalytics.Configuration;
using LXGaming.TorrentAnalytics.Services.Torrent.Client;
using LXGaming.TorrentAnalytics.Services.Torrent.Models;
using LXGaming.Hosting;
using LXGaming.TorrentAnalytics.Services.QBittorrent.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent;

[Service(ServiceLifetime.Singleton, typeof(ITorrentClientProvider))]
public class QBittorrentService(
    IConfiguration<Config> configuration,
    ILogger<QBittorrentService> logger,
    ISchedulerFactory schedulerFactory,
    IServiceProvider serviceProvider) : IHostedService, ITorrentClientProvider {

    public bool TorrentMetrics => configuration.Value?.QBittorrentCategory.TorrentMetrics == true;

    public bool TrackerMetrics => configuration.Value?.QBittorrentCategory.TrackerMetrics == true;

    public TorrentClientType Type => TorrentClientType.QBittorrent;

    public async Task StartAsync(CancellationToken cancellationToken) {
        var category = configuration.Value?.QBittorrentCategory;
        if (category == null) {
            throw new InvalidOperationException("qBittorrentCategory is unavailable");
        }

        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        if (!string.IsNullOrEmpty(category.Schedule)) {
            await scheduler.ScheduleJob(
                JobBuilder.Create<QBittorrentJob>().WithIdentity(QBittorrentJob.JobKey).Build(),
                TriggerBuilder.Create().WithCronSchedule(category.Schedule).Build(),
                cancellationToken);
        } else {
            logger.LogWarning("qBittorrent schedule has not been configured");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public ITorrentClient CreateClient(TorrentClientOptions options) {
        return new QBittorrentTorrentClient(options, serviceProvider);
    }
}