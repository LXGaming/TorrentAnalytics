using LXGaming.Hosting.Reflection;
using LXGaming.TorrentAnalytics.Services.Flood;
using LXGaming.TorrentAnalytics.Services.Torrent;
using LXGaming.TorrentAnalytics.Services.Torrent.Client;
using LXGaming.TorrentAnalytics.Services.Torrent.Utilities;
using LXGaming.TorrentAnalytics.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LXGaming.TorrentAnalytics.Tests.Services.Flood;

[Parallelizable]
public class FloodServiceTest : ServiceTestBase {

    private FloodTorrentClient? _torrentClient;

    public FloodServiceTest() {
        Services.AddSingleton<FloodService>();
        Services.AddSingleton<ITorrentClientProvider>(provider => provider.GetRequiredService<FloodService>());
        Services.AddService<TorrentService>();
        Services.AddLogging();
        Services.AddSchedulerFactory();
        Services.AddWebService();
    }

    [OneTimeSetUp]
    public void Setup() {
        var client = Provider.GetRequiredService<TorrentService>().GetClient<FloodTorrentClient>();
        if (client == null) {
            Assert.Ignore("Flood torrent client has not been configured");
        }

        _torrentClient = client;
    }

    [OneTimeTearDown]
    public void Teardown() {
        _torrentClient?.Dispose();
    }

    [Test]
    public Task DeserializeTorrentsAsync() => _torrentClient!.GetTorrentsAsync();
}