using LXGaming.Configuration.Generic;
using LXGaming.Hosting.Reflection;
using LXGaming.TorrentAnalytics.Configuration;
using LXGaming.TorrentAnalytics.Services.Flood;
using LXGaming.TorrentAnalytics.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LXGaming.TorrentAnalytics.Tests.Services.Flood;

[Parallelizable]
public class FloodServiceTest : ServiceTestBase {

    public FloodServiceTest() {
        Services.AddService<FloodService>();
        Services.AddLogging();
        Services.AddSchedulerFactory();
        Services.AddWebService();
    }

    [OneTimeSetUp]
    public void Setup() {
        var configuration = Provider.GetRequiredService<IConfiguration<Config>>();
        var category = configuration.Value?.FloodCategory;
        if (string.IsNullOrEmpty(category?.Address)) {
            Assert.Ignore("Flood address has not been configured");
        }
    }

    [Test]
    [Order(1)]
    public Task DeserializeAuthenticateAsync() => Provider.GetRequiredService<FloodService>().AuthenticateAsync();

    [Test]
    public Task DeserializeTorrentsAsync() => Provider.GetRequiredService<FloodService>().GetTorrentsAsync();
}