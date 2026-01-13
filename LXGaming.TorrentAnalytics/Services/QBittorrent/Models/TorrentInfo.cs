using System.Collections.Immutable;
using System.Text.Json.Serialization;
using LXGaming.TorrentAnalytics.Utilities.Json.Converters;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

public record TorrentInfo {

    [JsonPropertyName("added_on")]
    public long AddedOn { get; init; }

    [JsonPropertyName("amount_left")]
    public long AmountLeft { get; init; }

    [JsonPropertyName("auto_tmm")]
    public bool AutoTorrentManagement { get; init; }

    [JsonPropertyName("availability")]
    public double Availability { get; init; }

    [JsonPropertyName("category")]
    public required string Category { get; init; }

    /// <remarks>
    /// Added in 5.0.0
    /// </remarks>
    [JsonPropertyName("comment")]
    public string? Comment { get; init; }

    [JsonPropertyName("completed")]
    public long Completed { get; init; }

    [JsonPropertyName("completion_on")]
    public long CompletionOn { get; init; }

    [JsonPropertyName("content_path")]
    public required string ContentPath { get; init; }

    [JsonPropertyName("dl_limit")]
    public int DownloadLimit { get; init; }

    [JsonPropertyName("dlspeed")]
    public int DownloadSpeed { get; init; }

    [JsonPropertyName("download_path")]
    public required string DownloadPath { get; init; }

    [JsonPropertyName("downloaded")]
    public long Downloaded { get; init; }

    [JsonPropertyName("downloaded_session")]
    public long DownloadedSession { get; init; }

    [JsonPropertyName("eta")]
    public long Eta { get; init; }

    [JsonPropertyName("f_l_piece_prio")]
    public bool FirstLastPiecePriority { get; init; }

    [JsonPropertyName("force_start")]
    public bool ForceStart { get; init; }

    /// <remarks>
    /// Added in 5.0.0
    /// </remarks>
    [JsonPropertyName("has_metadata")]
    public bool? HasMetadata { get; init; }

    [JsonPropertyName("hash")]
    public required string Hash { get; init; }

    [JsonPropertyName("inactive_seeding_time_limit")]
    public int InactiveSeedingTimeLimit { get; init; }

    [JsonPropertyName("infohash_v1")]
    public required string InfoHashV1 { get; init; }

    [JsonPropertyName("infohash_v2")]
    public required string InfoHashV2 { get; init; }

    [JsonPropertyName("last_activity")]
    public long LastActivity { get; init; }

    [JsonPropertyName("magnet_uri")]
    public required string MagnetUri { get; init; }

    [JsonPropertyName("max_inactive_seeding_time")]
    public int MaxInactiveSeedingTime { get; init; }

    [JsonPropertyName("max_ratio")]
    public double MaxRatio { get; init; }

    [JsonPropertyName("max_seeding_time")]
    public int MaxSeedingTime { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("num_complete")]
    public int SeedsTotal { get; init; }

    [JsonPropertyName("num_incomplete")]
    public int LeechesTotal { get; init; }

    [JsonPropertyName("num_leechs")]
    public int LeechesConnected { get; init; }

    [JsonPropertyName("num_seeds")]
    public int SeedsConnected { get; init; }

    /// <remarks>
    /// Added in 5.0.0
    /// </remarks>
    [JsonPropertyName("popularity")]
    public double? Popularity { get; init; }

    [JsonPropertyName("priority")]
    public int Priority { get; init; }

    /// <remarks>
    /// Added in 5.0.0
    /// </remarks>
    [JsonPropertyName("private")]
    public bool? Private { get; init; }

    [JsonPropertyName("progress")]
    public double Progress { get; init; }

    [JsonPropertyName("ratio")]
    public double Ratio { get; init; }

    [JsonPropertyName("ratio_limit")]
    public double RatioLimit { get; init; }

    /// <remarks>
    /// Added in 5.0.0
    /// </remarks>
    [JsonPropertyName("reannounce")]
    public long? Reannounce { get; init; }

    /// <remarks>
    /// Added in 5.0.0
    /// </remarks>
    [JsonPropertyName("root_path")]
    public string? RootPath { get; init; }

    [JsonPropertyName("save_path")]
    public required string SavePath { get; init; }

    [JsonPropertyName("seeding_time")]
    public long SeedingTime { get; init; }

    [JsonPropertyName("seeding_time_limit")]
    public int SeedingTimeLimit { get; init; }

    [JsonPropertyName("seen_complete")]
    public long SeenComplete { get; init; }

    [JsonPropertyName("seq_dl")]
    public bool SequentialDownload { get; init; }

    [JsonPropertyName("size")]
    public long Size { get; init; }

    [JsonPropertyName("state")]
    public TorrentState State { get; init; }

    [JsonPropertyName("super_seeding")]
    public bool SuperSeeding { get; init; }

    [JsonPropertyName("tags")]
    [JsonConverter(typeof(TagsConverter))]
    public required ImmutableArray<string> Tags { get; init; }

    [JsonPropertyName("time_active")]
    public long TimeActive { get; init; }

    [JsonPropertyName("total_size")]
    public long TotalSize { get; init; }

    [JsonPropertyName("tracker")]
    public required string Tracker { get; init; }

    /// <remarks>
    /// Added in 5.1.0
    /// </remarks>
    [JsonPropertyName("trackers")]
    public ImmutableArray<TorrentTracker>? Trackers { get; init; }

    [JsonPropertyName("trackers_count")]
    public int TrackersCount { get; init; }

    [JsonPropertyName("up_limit")]
    public int UploadLimit { get; init; }

    [JsonPropertyName("uploaded")]
    public long Uploaded { get; init; }

    [JsonPropertyName("uploaded_session")]
    public long UploadedSession { get; init; }

    [JsonPropertyName("upspeed")]
    public int UploadSpeed { get; init; }
}