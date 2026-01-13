using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

public record TorrentTracker {

    [JsonPropertyName("msg")]
    public required string Message { get; init; }

    [JsonPropertyName("num_downloaded")]
    public int Downloaded { get; init; }

    [JsonPropertyName("num_leeches")]
    public int Leeches { get; init; }

    [JsonPropertyName("num_peers")]
    public int Peers { get; init; }

    [JsonPropertyName("num_seeds")]
    public int Seeds { get; init; }

    [JsonPropertyName("status")]
    public TrackerStatus Status { get; init; }

    [JsonPropertyName("tier")]
    public int Tier { get; init; }

    [JsonPropertyName("url")]
    public required string Url { get; init; }
}