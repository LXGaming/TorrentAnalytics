using System.Collections.Frozen;
using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Services.Flood.Models;

// https://github.com/jesec/flood/blob/b0968ef9c0cdd7ecabbbab78335706e2643e6bdd/shared/types/Torrent.ts#L61
public record TorrentListSummary {

    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("torrents")]
    public required FrozenDictionary<string, TorrentProperties> Torrents { get; init; }
}