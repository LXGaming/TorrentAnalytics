using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Configuration.Categories;

public class WebCategory {

    public const int DefaultPooledConnectionLifetime = 300000; // 5 Minutes
    public const int DefaultTimeout = 100000; // 100 Seconds

    [JsonPropertyName("pooledConnectionLifetime")]
    public int PooledConnectionLifetime { get; set; } = DefaultPooledConnectionLifetime;

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = DefaultTimeout;
}