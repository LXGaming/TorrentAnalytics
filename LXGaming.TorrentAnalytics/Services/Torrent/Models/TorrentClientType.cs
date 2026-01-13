using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.TorrentAnalytics.Services.Torrent.Models;

[JsonConverter(typeof(StringEnumConverter<TorrentClientType>))]
public enum TorrentClientType {

    None,
    Flood,
    QBittorrent
}