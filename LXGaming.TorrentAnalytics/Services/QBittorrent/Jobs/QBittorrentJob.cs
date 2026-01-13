using System.Collections.Immutable;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using LXGaming.TorrentAnalytics.Services.InfluxDb;
using LXGaming.TorrentAnalytics.Services.InfluxDb.Models;
using LXGaming.TorrentAnalytics.Services.QBittorrent.Models;
using LXGaming.TorrentAnalytics.Services.QBittorrent.Utilities;
using LXGaming.TorrentAnalytics.Services.Torrent;
using LXGaming.TorrentAnalytics.Services.Torrent.Utilities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Jobs;

[DisallowConcurrentExecution]
public class QBittorrentJob(
    InfluxDbService influxDbService,
    ILogger<QBittorrentJob> logger,
    QBittorrentService qBittorrentService,
    TorrentService torrentService) : IJob {

    public static readonly JobKey JobKey = JobKey.Create(nameof(QBittorrentJob));

    public async Task Execute(IJobExecutionContext context) {
        if (influxDbService.Client == null) {
            throw new InvalidOperationException("InfluxDBClient is unavailable");
        }

        foreach (var torrentClient in torrentService.GetClients<QBittorrentTorrentClient>()) {
            try {
                await ExecuteAsync(torrentClient, context.ScheduledFireTimeUtc ?? context.FireTimeUtc);
            } catch (Exception ex) {
                logger.LogWarning(ex, "Encountered an error while executing {Client}", torrentClient);
            }
        }
    }

    private async Task ExecuteAsync(QBittorrentTorrentClient torrentClient, DateTimeOffset timestamp) {
        var torrents = await torrentClient.GetTorrentInfosAsync(includeTrackers: true);
        if (torrents.Length == 0) {
            return;
        }

        if (qBittorrentService.TorrentMetrics) {
            var points = GetTorrentMetrics(torrents, timestamp);
            await influxDbService.Client!.GetWriteApiAsync().WritePointsAsync(points);
        }

        if (qBittorrentService.TrackerMetrics) {
            var points = GetTrackerMetrics(torrents, timestamp);
            await influxDbService.Client!.GetWriteApiAsync().WritePointsAsync(points);
        }
    }

    private static List<PointData> GetTorrentMetrics(ImmutableArray<TorrentInfo> torrents, DateTimeOffset timestamp) {
        var points = new List<PointData>(torrents.Length);
        foreach (var torrent in torrents) {
            var trackers = torrent.Trackers != null
                ? string.Join(',', GetTrackerStrings(torrent.Trackers).Order())
                : GetTrackerString(torrent.Tracker);
            var crossSeeds = torrents
                .Where(model => string.Equals(model.Name, torrent.Name))
                .Count(model => !string.Equals(model.Hash, torrent.Hash));

            var point = PointData.Builder
                .Measurement("qbittorrent_torrent")
                .Tag("id", torrent.Hash)
                .Tag("name", torrent.Name)
                .Tag("trackers", trackers)
                .Field("amount_left", torrent.AmountLeft)
                .Field("auto_torrent_management", torrent.AutoTorrentManagement)
                .Field("availability", torrent.Availability)
                .Field("category", torrent.Category)
                .Field("completed", torrent.Completed)
                .Field("cross_seeds", crossSeeds)
                .Field("download_limit", torrent.DownloadLimit)
                .Field("download_speed", torrent.DownloadSpeed)
                .Field("downloaded_session", torrent.DownloadedSession)
                .Field("downloaded", torrent.Downloaded)
                .Field("eta", torrent.Eta)
                .Field("first_last_piece_priority", torrent.FirstLastPiecePriority)
                .Field("force_start", torrent.ForceStart)
                .Field("has_metadata", torrent.HasMetadata ?? false)
                .Field("inactive_seeding_time_limit", torrent.InactiveSeedingTimeLimit)
                .Field("is_checking_downloading", torrent.State is TorrentState.CheckingDownloading)
                .Field("is_checking_resume_data", torrent.State is TorrentState.CheckingResumeData)
                .Field("is_checking_uploading", torrent.State is TorrentState.CheckingUploading)
                .Field("is_downloading_metadata", torrent.State is TorrentState.DownloadingMetadata)
                .Field("is_downloading", torrent.State is TorrentState.Downloading)
                .Field("is_error", torrent.State is TorrentState.Error)
                .Field("is_forced_downloading_metadata", torrent.State is TorrentState.ForcedDownloadingMetadata)
                .Field("is_forced_downloading", torrent.State is TorrentState.ForcedDownloading)
                .Field("is_forced_uploading", torrent.State is TorrentState.ForcedUploading)
                .Field("is_missing_files", torrent.State is TorrentState.MissingFiles)
                .Field("is_moving", torrent.State is TorrentState.Moving)
                .Field("is_queued_downloading", torrent.State is TorrentState.QueuedDownloading)
                .Field("is_queued_uploading", torrent.State is TorrentState.QueuedUploading)
                .Field("is_stalled_downloading", torrent.State is TorrentState.StalledDownloading)
                .Field("is_stalled_uploading", torrent.State is TorrentState.StalledUploading)
                .Field("is_stopped_downloading", torrent.State is TorrentState.PausedDownloading or TorrentState.StoppedDownloading)
                .Field("is_stopped_uploading", torrent.State is TorrentState.PausedUploading or TorrentState.StoppedUploading)
                .Field("is_uploading", torrent.State is TorrentState.Uploading)
                .Field("leeches_connected", torrent.LeechesConnected)
                .Field("leeches_total", torrent.LeechesTotal)
                .Field("max_inactive_seeding_time", torrent.MaxInactiveSeedingTime)
                .Field("max_ratio", torrent.MaxRatio)
                .Field("max_seeding_time", torrent.MaxSeedingTime)
                .Field("popularity", torrent.Popularity ?? -1.0D)
                .Field("priority", torrent.Priority)
                .Field("private", torrent.Private ?? true) // Fail-safe, don't assume it's public.
                .Field("progress", torrent.Progress)
                .Field("ratio_limit", torrent.RatioLimit)
                .Field("ratio", torrent.Ratio)
                .Field("reannounce", torrent.Reannounce ?? -1L)
                .Field("seeding_time_limit", torrent.SeedingTimeLimit)
                .Field("seeding_time", torrent.SeedingTime)
                .Field("seeds_connected", torrent.SeedsConnected)
                .Field("seeds_total", torrent.SeedsTotal)
                .Field("sequential_download", torrent.SequentialDownload)
                .Field("size", torrent.Size)
                .Field("super_seeding", torrent.SuperSeeding)
                .Field("time_active", torrent.TimeActive)
                .Field("total_size", torrent.TotalSize)
                .Field("trackers_count", torrent.TrackersCount)
                .Field("upload_limit", torrent.UploadLimit)
                .Field("upload_speed", torrent.UploadSpeed)
                .Field("uploaded_session", torrent.UploadedSession)
                .Field("uploaded", torrent.Uploaded)
                .Timestamp(timestamp, WritePrecision.S)
                .ToPointData();

            points.Add(point);
        }

        return points;
    }

    private static List<PointData> GetTrackerMetrics(ImmutableArray<TorrentInfo> torrents, DateTimeOffset timestamp) {
        var trackers = new Dictionary<string, Fields>();
        foreach (var torrent in torrents) {
            var id = torrent.Trackers != null
                ? string.Join(',', GetTrackerStrings(torrent.Trackers).Order())
                : GetTrackerString(torrent.Tracker);
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

            var crossSeeds = torrents
                .Where(model => string.Equals(model.Name, torrent.Name))
                .Any(model => !string.Equals(model.Hash, torrent.Hash));

            fields
                .AddOrIncrement("amount_left", torrent.AmountLeft)
                .AddOrIncrement("auto_torrent_management", torrent.AutoTorrentManagement ? 1 : 0)
                .AddOrIncrement("availability", torrent.Availability)
                .AddOrIncrement("completed", torrent.Completed)
                .AddOrIncrement("cross_seeds", crossSeeds ? 1 : 0)
                .AddOrIncrement("download_speed", torrent.DownloadSpeed)
                .AddOrIncrement("downloaded_session", torrent.DownloadedSession)
                .AddOrIncrement("downloaded", torrent.Downloaded)
                .AddOrIncrement("eta", torrent.Eta)
                .AddOrIncrement("first_last_piece_priority", torrent.FirstLastPiecePriority ? 1 : 0)
                .AddOrIncrement("force_start", torrent.ForceStart ? 1 : 0)
                .AddOrIncrement("has_metadata", torrent.HasMetadata == true ? 1 : 0)
                .AddOrIncrement("is_checking_downloading", torrent.State is TorrentState.CheckingDownloading ? 1 : 0)
                .AddOrIncrement("is_checking_resume_data", torrent.State is TorrentState.CheckingResumeData ? 1 : 0)
                .AddOrIncrement("is_checking_uploading", torrent.State is TorrentState.CheckingUploading ? 1 : 0)
                .AddOrIncrement("is_downloading_metadata", torrent.State is TorrentState.DownloadingMetadata ? 1 : 0)
                .AddOrIncrement("is_downloading", torrent.State is TorrentState.Downloading ? 1 : 0)
                .AddOrIncrement("is_error", torrent.State is TorrentState.Error ? 1 : 0)
                .AddOrIncrement("is_forced_downloading_metadata", torrent.State is TorrentState.ForcedDownloadingMetadata ? 1 : 0)
                .AddOrIncrement("is_forced_downloading", torrent.State is TorrentState.ForcedDownloading ? 1 : 0)
                .AddOrIncrement("is_forced_uploading", torrent.State is TorrentState.ForcedUploading ? 1 : 0)
                .AddOrIncrement("is_missing_files", torrent.State is TorrentState.MissingFiles ? 1 : 0)
                .AddOrIncrement("is_moving", torrent.State is TorrentState.Moving ? 1 : 0)
                .AddOrIncrement("is_queued_downloading", torrent.State is TorrentState.QueuedDownloading ? 1 : 0)
                .AddOrIncrement("is_queued_uploading", torrent.State is TorrentState.QueuedUploading ? 1 : 0)
                .AddOrIncrement("is_stalled_downloading", torrent.State is TorrentState.StalledDownloading ? 1 : 0)
                .AddOrIncrement("is_stalled_uploading", torrent.State is TorrentState.StalledUploading ? 1 : 0)
                .AddOrIncrement("is_stopped_downloading", torrent.State is TorrentState.PausedDownloading or TorrentState.StoppedDownloading ? 1 : 0)
                .AddOrIncrement("is_stopped_uploading", torrent.State is TorrentState.PausedUploading or TorrentState.StoppedUploading ? 1 : 0)
                .AddOrIncrement("is_uploading", torrent.State is TorrentState.Uploading ? 1 : 0)
                .AddOrIncrement("leeches_connected", torrent.LeechesConnected)
                .AddOrIncrement("leeches_total", torrent.LeechesTotal)
                .AddOrIncrement("popularity", torrent.Popularity ?? 0.0D)
                .AddOrIncrement("progress", torrent.Progress)
                .AddOrIncrement("ratio", torrent.Ratio)
                .AddOrIncrement("seeding_time", torrent.SeedingTime)
                .AddOrIncrement("seeds_connected", torrent.SeedsConnected)
                .AddOrIncrement("seeds_total", torrent.SeedsTotal)
                .AddOrIncrement("sequential_download", torrent.SequentialDownload ? 1 : 0)
                .AddOrIncrement("size", torrent.Size)
                .AddOrIncrement("super_seeding", torrent.SuperSeeding ? 1 : 0)
                .AddOrIncrement("time_active", torrent.TimeActive)
                .AddOrIncrement("torrent", 1)
                .AddOrIncrement("total_size", torrent.TotalSize)
                .AddOrIncrement("upload_speed", torrent.UploadSpeed)
                .AddOrIncrement("uploaded_session", torrent.UploadedSession)
                .AddOrIncrement("uploaded", torrent.Uploaded)
                .AddOrIncrement(torrent.Private ?? true ? "private" : "public", 1); // Fail-safe, don't assume it's public.
        }

        var points = new List<PointData>(trackers.Count);
        foreach (var (id, fields) in trackers) {
            var builder = PointData.Builder
                .Measurement("qbittorrent_tracker")
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

    private static HashSet<string> GetTrackerStrings(IEnumerable<TorrentTracker> torrentTrackers) {
        return torrentTrackers
            .Where(model => model.Status != TrackerStatus.Disabled)
            .Where(model => !model.IsDistributedHashTable())
            .Where(model => !model.IsLocalServiceDiscovery())
            .Where(model => !model.IsPeerExchange())
            .Select(model => GetTrackerString(model.Url))
            .Where(trackerString => !string.IsNullOrEmpty(trackerString))
            .Select(trackerString => trackerString!)
            .ToHashSet();
    }

    private static string? GetTrackerString(string uriString) {
        return Uri.TryCreate(uriString, UriKind.Absolute, out var uri) ? uri.Host : null;
    }
}