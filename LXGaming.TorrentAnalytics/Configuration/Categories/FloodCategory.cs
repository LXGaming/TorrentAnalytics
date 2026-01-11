using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Configuration.Categories;

public class FloodCategory {

    [JsonPropertyName("schedule")]
    public string Schedule { get; init; } = "0 */10 * * * ?";
}