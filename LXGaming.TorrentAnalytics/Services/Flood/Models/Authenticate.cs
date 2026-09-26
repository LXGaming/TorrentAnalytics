using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Services.Flood.Models;

// https://github.com/jesec/flood/blob/b0968ef9c0cdd7ecabbbab78335706e2643e6bdd/shared/schema/api/auth.ts#L27
public record Authenticate {

    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("level")]
    public AccessLevel Level { get; init; }
}