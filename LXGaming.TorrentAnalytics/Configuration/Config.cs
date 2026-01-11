using System.Text.Json.Serialization;
using LXGaming.TorrentAnalytics.Configuration.Categories;

namespace LXGaming.TorrentAnalytics.Configuration;

public class Config {

    [JsonPropertyName("flood")]
    public FloodCategory FloodCategory { get; init; } = new();

    [JsonPropertyName("influxDb")]
    public InfluxDbCategory InfluxDbCategory { get; init; } = new();

    [JsonPropertyName("quartz")]
    public QuartzCategory QuartzCategory { get; init; } = new();

    [JsonPropertyName("torrent")]
    public TorrentCategory TorrentCategory { get; init; } = new();

    [JsonPropertyName("web")]
    public WebCategory WebCategory { get; init; } = new();
}