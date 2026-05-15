using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

public record AppBuildInfo {

    [JsonPropertyName("bitness")]
    public int Bitness { get; init; }

    [JsonPropertyName("boost")]
    public required string Boost { get; init; }

    [JsonPropertyName("libtorrent")]
    public required string Libtorrent { get; init; }

    [JsonPropertyName("openssl")]
    public required string OpenSsl { get; init; }

    /// <remarks>
    /// Added in 5.0.0
    /// </remarks>
    [JsonPropertyName("platform")]
    public string? Platform { get; init; }

    [JsonPropertyName("qt")]
    public required string Qt { get; init; }

    [JsonPropertyName("zlib")]
    public required string Zlib { get; init; }
}