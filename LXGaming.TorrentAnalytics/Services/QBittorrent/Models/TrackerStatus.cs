namespace LXGaming.TorrentAnalytics.Services.QBittorrent.Models;

public enum TrackerStatus {

    Disabled = 0,

    NotContacted = 1,

    Working = 2,

    /// <remarks>
    /// 5.1.4, for 5.2.0+ use <see cref="TorrentTracker.Updating"/>
    /// </remarks>
    Updating = 3,

    NotWorking = 4,

    TrackerError = 5,

    Unreachable = 6
}