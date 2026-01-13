using System.Net;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using LXGaming.TorrentAnalytics.Services.Flood.Models;
using LXGaming.TorrentAnalytics.Services.InfluxDb;
using LXGaming.TorrentAnalytics.Services.InfluxDb.Models;
using LXGaming.TorrentAnalytics.Services.Torrent;
using LXGaming.TorrentAnalytics.Services.Torrent.Utilities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.TorrentAnalytics.Services.Flood.Jobs;

[DisallowConcurrentExecution]
public class FloodJob(
    FloodService floodService,
    InfluxDbService influxDbService,
    ILogger<FloodJob> logger,
    TorrentService torrentService) : IJob {

    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodJob));

    public async Task Execute(IJobExecutionContext context) {
        if (influxDbService.Client == null) {
            throw new InvalidOperationException("InfluxDBClient is unavailable");
        }

        foreach (var torrentClient in torrentService.GetClients<FloodTorrentClient>()) {
            try {
                await ExecuteAsync(torrentClient, context.ScheduledFireTimeUtc ?? context.FireTimeUtc);
            } catch (Exception ex) {
                logger.LogWarning(ex, "Encountered an error while executing {Client}", torrentClient);
            }
        }
    }

    private async Task ExecuteAsync(FloodTorrentClient torrentClient, DateTimeOffset timestamp) {
        TorrentListSummary torrentListSummary;
        try {
            torrentListSummary = await torrentClient.GetTorrentsAsync();
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

        if (floodService.TorrentMetrics) {
            var points = GetTorrentMetrics(torrentListSummary, timestamp);
            await influxDbService.Client!.GetWriteApiAsync().WritePointsAsync(points);
        }

        if (floodService.TrackerMetrics) {
            var points = GetTrackerMetrics(torrentListSummary, timestamp);
            await influxDbService.Client!.GetWriteApiAsync().WritePointsAsync(points);
        }
    }

    private static List<PointData> GetTorrentMetrics(TorrentListSummary torrentListSummary, DateTimeOffset timestamp) {
        var points = new List<PointData>(torrentListSummary.Torrents.Count);
        foreach (var (_, value) in torrentListSummary.Torrents) {
            var trackers = string.Join(',', value.TrackerUris.Order());
            var crossSeeds = torrentListSummary.Torrents
                .Select(model => model.Value)
                .Where(model => string.Equals(model.Name, value.Name))
                .Count(model => !string.Equals(model.Hash, value.Hash));

            var point = PointData.Builder
                .Measurement("flood_torrent")
                .Tag("id", value.Hash)
                .Tag("name", value.Name)
                .Tag("trackers", trackers)
                .Field("bytes_done", value.BytesDone)
                .Field("cross_seeds", crossSeeds)
                .Field("down_rate", value.DownRate)
                .Field("down_total", value.DownTotal)
                .Field("eta", value.Eta)
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
                .Timestamp(timestamp, WritePrecision.S)
                .ToPointData();

            points.Add(point);
        }

        return points;
    }

    private static List<PointData> GetTrackerMetrics(TorrentListSummary torrentListSummary, DateTimeOffset timestamp) {
        var trackers = new Dictionary<string, Fields>();
        foreach (var (_, value) in torrentListSummary.Torrents) {
            var id = string.Join(',', value.TrackerUris.Order());
            if (string.IsNullOrEmpty(id)) {
                continue;
            }

            Fields fields;
            if (trackers.TryGetValue(id, out var existingFields)) {
                fields = existingFields;
            } else {
                fields = new Fields();
                trackers[id] = fields;
            }

            var crossSeeds = torrentListSummary.Torrents
                .Select(model => model.Value)
                .Where(model => string.Equals(model.Name, value.Name))
                .Any(model => !string.Equals(model.Hash, value.Hash));

            fields
                .AddOrIncrement("bytes_done", value.BytesDone)
                .AddOrIncrement("cross_seeds", crossSeeds ? 1 : 0)
                .AddOrIncrement("down_rate", value.DownRate)
                .AddOrIncrement("down_total", value.DownTotal)
                .AddOrIncrement("eta", value.Eta)
                .AddOrIncrement("is_active", value.Status.Contains(TorrentStatus.Active) ? 1 : 0)
                .AddOrIncrement("is_checking", value.Status.Contains(TorrentStatus.Checking) ? 1 : 0)
                .AddOrIncrement("is_complete", value.Status.Contains(TorrentStatus.Complete) ? 1 : 0)
                .AddOrIncrement("is_downloading", value.Status.Contains(TorrentStatus.Downloading) ? 1 : 0)
                .AddOrIncrement("is_error", value.Status.Contains(TorrentStatus.Error) ? 1 : 0)
                .AddOrIncrement("is_inactive", value.Status.Contains(TorrentStatus.Inactive) ? 1 : 0)
                .AddOrIncrement("is_initial_seeding", value.IsInitialSeeding ? 1 : 0)
                .AddOrIncrement("is_seeding", value.Status.Contains(TorrentStatus.Seeding) ? 1 : 0)
                .AddOrIncrement("is_sequential", value.IsSequential ? 1 : 0)
                .AddOrIncrement("is_stopped", value.Status.Contains(TorrentStatus.Stopped) ? 1 : 0)
                .AddOrIncrement("peers_connected", value.PeersConnected)
                .AddOrIncrement("peers_total", value.PeersTotal)
                .AddOrIncrement("percent_complete", value.PercentComplete)
                .AddOrIncrement("ratio", value.Ratio)
                .AddOrIncrement("seeds_connected", value.SeedsConnected)
                .AddOrIncrement("seeds_total", value.SeedsTotal)
                .AddOrIncrement("size_bytes", value.SizeBytes)
                .AddOrIncrement("torrent", 1)
                .AddOrIncrement("up_rate", value.UpRate)
                .AddOrIncrement("up_total", value.UpTotal)
                .AddOrIncrement(value.IsPrivate ? "is_private" : "is_public", 1);
        }

        var points = new List<PointData>(trackers.Count);
        foreach (var (id, fields) in trackers) {
            var builder = PointData.Builder
                .Measurement("flood_tracker")
                .Tag("id", id);

            foreach (var (key, value) in fields.FloatingPoint) {
                builder.Field(key, value);
            }

            foreach (var (key, value) in fields.Integral) {
                builder.Field(key, value);
            }

            builder.Timestamp(timestamp, WritePrecision.S);
            points.Add(builder.ToPointData());
        }

        return points;
    }
}