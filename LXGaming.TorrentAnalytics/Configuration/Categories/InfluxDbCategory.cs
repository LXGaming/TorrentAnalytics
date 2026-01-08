using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Configuration.Categories;

public class InfluxDbCategory {

    [JsonPropertyName("url")]
    public string Url { get; init; } = "";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("bucket")]
    public string? Bucket { get; init; }

    [JsonPropertyName("organization")]
    public string? Organization { get; init; }
}