namespace LXGaming.TorrentAnalytics.Services.Flood.Models;

// https://github.com/jesec/flood/blob/77f4bc7267331f2c731c47dd62b570d4f0bf0c1d/shared/types/Torrent.ts#L12
public enum TorrentPriority {

    DoNotDownload = 0,
    Low = 1,
    Normal = 2,
    High = 3
}