using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

[JsonConverter(typeof(StringEnumConverter<TorrentState>))]
public enum TorrentState {

    [JsonPropertyName("unknown")]
    Unknown = 0,

    [JsonPropertyName("forcedDL")]
    ForcedDownloading,

    [JsonPropertyName("downloading")]
    Downloading,

    [JsonPropertyName("forcedMetaDL")]
    ForcedDownloadingMetadata,

    [JsonPropertyName("metaDL")]
    DownloadingMetadata,

    [JsonPropertyName("stalledDL")]
    StalledDownloading,

    [JsonPropertyName("forcedUP")]
    ForcedUploading,

    [JsonPropertyName("uploading")]
    Uploading,

    [JsonPropertyName("stalledUP")]
    StalledUploading,

    [JsonPropertyName("checkingResumeData")]
    CheckingResumeData,

    [JsonPropertyName("queuedDL")]
    QueuedDownloading,

    [JsonPropertyName("queuedUP")]
    QueuedUploading,

    [JsonPropertyName("checkingUP")]
    CheckingUploading,

    [JsonPropertyName("checkingDL")]
    CheckingDownloading,

    /// <remarks>
    /// 4.6.7, for 5.0.0+ use <see cref="StoppedDownloading"/>
    /// </remarks>
    [JsonPropertyName("pausedDL")]
    PausedDownloading,

    /// <remarks>
    /// 5.0.0+, for 4.6.7 use <see cref="PausedDownloading"/>
    /// </remarks>
    [JsonPropertyName("stoppedDL")]
    StoppedDownloading,

    /// <remarks>
    /// 4.6.7, for 5.0.0+ use <see cref="StoppedUploading"/>
    /// </remarks>
    [JsonPropertyName("pausedUP")]
    PausedUploading,

    /// <remarks>
    /// 5.0.0+, for 4.6.7 use <see cref="PausedUploading"/>
    /// </remarks>
    [JsonPropertyName("stoppedUP")]
    StoppedUploading,

    [JsonPropertyName("moving")]
    Moving,

    [JsonPropertyName("missingFiles")]
    MissingFiles,

    [JsonPropertyName("error")]
    Error
}