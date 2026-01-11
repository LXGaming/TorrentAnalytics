using System.Text.Json.Serialization;
using LXGaming.TorrentAnalytics.Services.Torrent.Client;

namespace LXGaming.TorrentAnalytics.Configuration.Categories;

public class TorrentCategory {

    [JsonPropertyName("clients")]
    public List<TorrentClientOptions> Clients { get; set; } = [];
}