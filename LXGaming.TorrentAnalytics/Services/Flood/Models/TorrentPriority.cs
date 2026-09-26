namespace LXGaming.TorrentAnalytics.Services.Flood.Models;

// https://github.com/jesec/flood/blob/b0968ef9c0cdd7ecabbbab78335706e2643e6bdd/shared/types/Torrent.ts#L12
public enum TorrentPriority {

    DoNotDownload = 0,
    Low = 1,
    Normal = 2,
    High = 3
}