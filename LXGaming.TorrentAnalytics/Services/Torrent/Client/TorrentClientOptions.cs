using System.Text.Json.Serialization;
using LXGaming.TorrentAnalytics.Services.Torrent.Models;

namespace LXGaming.TorrentAnalytics.Services.Torrent.Client;

public class TorrentClientOptions {

    [JsonPropertyName("type")]
    public TorrentClientType Type { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("address")]
    public string Address { get; set; } = "";

    [JsonPropertyName("bypassAuthentication")]
    public bool BypassAuthentication { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = "";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "";

    [JsonPropertyName("additionalHeaders")]
    public Dictionary<string, string> AdditionalHeaders { get; set; } = new();

    public override string ToString() {
        var name = !string.IsNullOrWhiteSpace(Name) ? Name : "No Name";
        return $"{name} ({Type})";
    }
}