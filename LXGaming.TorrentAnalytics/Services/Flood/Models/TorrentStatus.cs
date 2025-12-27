using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.TorrentAnalytics.Services.Flood.Models;

// https://github.com/jesec/flood/blob/77f4bc7267331f2c731c47dd62b570d4f0bf0c1d/shared/constants/torrentStatusMap.ts
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

    [JsonPropertyName("error")]
    Error = 7
}