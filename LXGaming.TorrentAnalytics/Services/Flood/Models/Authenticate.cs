using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Services.Flood.Models;

// https://github.com/jesec/flood/blob/77f4bc7267331f2c731c47dd62b570d4f0bf0c1d/shared/schema/api/auth.ts#L17
public record Authenticate {

    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("level")]
    public AccessLevel Level { get; init; }
}