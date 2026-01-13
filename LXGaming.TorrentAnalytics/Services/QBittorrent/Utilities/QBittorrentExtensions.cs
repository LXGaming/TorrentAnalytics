using LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Utilities;

public static class QBittorrentExtensions {

    public static bool IsDistributedHashTable(this TorrentTracker torrentTracker) {
        return string.Equals(torrentTracker.Url, QBittorrentConstants.Trackers.DistributedHashTable);
    }

    public static bool IsLocalServiceDiscovery(this TorrentTracker torrentTracker) {
        return string.Equals(torrentTracker.Url, QBittorrentConstants.Trackers.LocalServiceDiscovery);
    }

    public static bool IsPeerExchange(this TorrentTracker torrentTracker) {
        return string.Equals(torrentTracker.Url, QBittorrentConstants.Trackers.PeerExchange);
    }
}