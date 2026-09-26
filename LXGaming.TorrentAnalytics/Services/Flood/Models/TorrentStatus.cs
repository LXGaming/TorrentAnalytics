using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.TorrentAnalytics.Services.Flood.Models;

// https://github.com/jesec/flood/blob/b0968ef9c0cdd7ecabbbab78335706e2643e6bdd/shared/constants/torrentStatusMap.ts
[JsonConverter(typeof(StringEnumConverter<TorrentStatus>))]
public enum TorrentStatus {

    [JsonPropertyName("downloading")]
    Downloading = 0,

    [JsonPropertyName("seeding")]
    Seeding = 1,

    [JsonPropertyName("checking")]
    Checking = 2,

    [JsonPropertyName("complete")]
    Complete = 3,

    [JsonPropertyName("stopped")]
    Stopped = 4,

    [JsonPropertyName("active")]
    Active = 5,

    [JsonPropertyName("inactive")]
    Inactive = 6,

    [JsonPropertyName("warning")]
    Warning = 7,

    [JsonPropertyName("error")]
    Error = 8,

    [JsonPropertyName("moving")]
    Moving = 9,
}