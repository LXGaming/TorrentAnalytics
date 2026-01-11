using LXGaming.TorrentAnalytics.Services.Torrent.Client;

namespace LXGaming.TorrentAnalytics.Services.Torrent.Utilities;

public static class TorrentExtensions {

    public static T? GetClient<T>(this TorrentService torrentService) where T : ITorrentClient {
        return (T?) torrentService.GetClient(typeof(T));
    }

    public static T GetRequiredClient<T>(this TorrentService torrentService) where T : ITorrentClient {
        var client = torrentService.GetClient<T>();
        if (client == null) {
            throw new InvalidOperationException($"No torrent client for '{typeof(T).FullName}' has been registered.");
        }

        return client;
    }

    public static IEnumerable<T> GetClients<T>(this TorrentService torrentService) where T : ITorrentClient {
        return torrentService.GetClients(typeof(T)).Cast<T>();
    }
}