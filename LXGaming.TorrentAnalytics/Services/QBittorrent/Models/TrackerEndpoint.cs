using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

public record TrackerEndpoint {

    [JsonPropertyName("bt_version")]
    public int BitTorrentVersion { get; init; }

    [JsonPropertyName("min_announce")]
    public long MinimumAnnounce { get; init; }

    [JsonPropertyName("msg")]
    public required string Message { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("next_announce")]
    public long NextAnnounce { get; init; }

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

    [JsonPropertyName("updating")]
    public bool Updating { get; init; }
}