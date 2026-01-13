using LXGaming.Hosting.Reflection;
using LXGaming.TorrentAnalytics.Services.QBittorrent;
using LXGaming.TorrentAnalytics.Services.Torrent;
using LXGaming.TorrentAnalytics.Services.Torrent.Client;
using LXGaming.TorrentAnalytics.Services.Torrent.Utilities;
using LXGaming.TorrentAnalytics.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LXGaming.TorrentAnalytics.Tests.Services.QBittorrent;

[Parallelizable]
public class QBittorrentServiceTest : ServiceTestBase {

    private QBittorrentTorrentClient? _torrentClient;

    public QBittorrentServiceTest() {
        Services.AddSingleton<QBittorrentService>();
        Services.AddSingleton<ITorrentClientProvider>(provider => provider.GetRequiredService<QBittorrentService>());
        Services.AddService<TorrentService>();
        Services.AddLogging();
        Services.AddSchedulerFactory();
        Services.AddWebService();
    }

    [OneTimeSetUp]
    public void Setup() {
        var client = Provider.GetRequiredService<TorrentService>().GetClient<QBittorrentTorrentClient>();
        if (client == null) {
            Assert.Ignore("qBittorrent torrent client has not been configured");
        }

        _torrentClient = client;
    }

    [OneTimeTearDown]
    public void Teardown() {
        _torrentClient?.Dispose();
    }

    [Test]
    public Task DeserializeAppVersionAsync() => _torrentClient!.GetAppVersionAsync();

    [Test]
    public Task DeserializeAppWebApiVersionAsync() => _torrentClient!.GetAppWebApiVersionAsync();

    [Test]
    public Task DeserializeAppBuildInfoAsync() => _torrentClient!.GetAppBuildInfoAsync();

    [Test]
    public Task DeserializeTorrentInfosAsync() => _torrentClient!.GetTorrentInfosAsync(includeTrackers: true);
}