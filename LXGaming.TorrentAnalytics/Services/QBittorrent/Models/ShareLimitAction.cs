using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

[JsonConverter(typeof(StringEnumConverter<ShareLimitAction>))]
public enum ShareLimitAction {

    [JsonPropertyName("Default")]
    Default = -1,

    [JsonPropertyName("Stop")]
    Stop = 0,

    [JsonPropertyName("Remove")]
    Remove = 1,

    [JsonPropertyName("EnableSuperSeeding")]
    EnableSuperSeeding = 2,

    [JsonPropertyName("RemoveWithContent")]
    RemoveWithContent = 3
}