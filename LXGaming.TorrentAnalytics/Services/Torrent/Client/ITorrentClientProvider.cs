using LXGaming.TorrentAnalytics.Services.Torrent.Models;

namespace LXGaming.TorrentAnalytics.Services.Torrent.Client;

public interface ITorrentClientProvider {

    TorrentClientType Type { get; }

    ITorrentClient CreateClient(TorrentClientOptions options);
}