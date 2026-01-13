using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Configuration.Categories;

public class QBittorrentCategory {

    [JsonPropertyName("schedule")]
    public string Schedule { get; init; } = "0 */10 * * * ?";

    [JsonPropertyName("torrentMetrics")]
    public bool TorrentMetrics { get; init; } = true;

    [JsonPropertyName("trackerMetrics")]
    public bool TrackerMetrics { get; init; } = true;
}