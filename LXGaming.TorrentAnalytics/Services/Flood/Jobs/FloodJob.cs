using System.Net;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using LXGaming.TorrentAnalytics.Services.Flood.Models;
using LXGaming.TorrentAnalytics.Services.InfluxDb;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.TorrentAnalytics.Services.Flood.Jobs;

[DisallowConcurrentExecution]
[PersistJobDataAfterExecution]
public class FloodJob(FloodService floodService, InfluxDbService influxDbService, ILogger<FloodJob> logger) : IJob {

    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodJob));

    public async Task Execute(IJobExecutionContext context) {
        if (influxDbService.Client == null) {
            throw new InvalidOperationException("InfluxDBClient is unavailable");
        }

        TorrentListSummary torrentListSummary;
        try {
            torrentListSummary = await floodService.EnsureAuthenticatedAsync(floodService.GetTorrentsAsync);
        } catch (HttpRequestException ex) {
            if (ex is not { StatusCode: HttpStatusCode.InternalServerError }) {
                throw;
            }

            logger.LogWarning("Encountered an Internal Server Error, check Flood for more details");
            return;
        }

        if (torrentListSummary.Torrents.Count == 0) {
            return;
        }

        var points = new List<PointData>(torrentListSummary.Torrents.Count);
        foreach (var (_, value) in torrentListSummary.Torrents) {
            var point = PointData.Builder
                .Measurement("torrent")
                .Tag("id", value.Hash)
                .Tag("name", value.Name)
                .Tag("trackers", string.Join(',', value.TrackerUris.Order()))
                .Field("bytes_done", value.BytesDone)
                .Field("down_rate", value.DownRate)
                .Field("down_total", value.DownTotal)
                .Field("is_active", value.Status.Contains(TorrentStatus.Active))
                .Field("is_checking", value.Status.Contains(TorrentStatus.Checking))
                .Field("is_complete", value.Status.Contains(TorrentStatus.Complete))
                .Field("is_downloading", value.Status.Contains(TorrentStatus.Downloading))
                .Field("is_error", value.Status.Contains(TorrentStatus.Error))
                .Field("is_inactive", value.Status.Contains(TorrentStatus.Inactive))
                .Field("is_initial_seeding", value.IsInitialSeeding)
                .Field("is_private", value.IsPrivate)
                .Field("is_seeding", value.Status.Contains(TorrentStatus.Seeding))
                .Field("is_sequential", value.IsSequential)
                .Field("is_stopped", value.Status.Contains(TorrentStatus.Stopped))
                .Field("message", value.Message)
                .Field("peers_connected", value.PeersConnected)
                .Field("peers_total", value.PeersTotal)
                .Field("percent_complete", value.PercentComplete)
                .Field("priority", (int) value.Priority)
                .Field("ratio", value.Ratio)
                .Field("seeds_connected", value.SeedsConnected)
                .Field("seeds_total", value.SeedsTotal)
                .Field("size_bytes", value.SizeBytes)
                .Field("up_rate", value.UpRate)
                .Field("up_total", value.UpTotal)
                .Timestamp(context.ScheduledFireTimeUtc ?? context.FireTimeUtc, WritePrecision.S)
                .ToPointData();

            points.Add(point);
        }

        await influxDbService.Client.GetWriteApiAsync().WritePointsAsync(points);
    }
}