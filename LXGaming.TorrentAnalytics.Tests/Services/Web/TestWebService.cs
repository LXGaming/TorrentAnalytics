using System.Text.Json;
using System.Text.Json.Serialization;
using LXGaming.Configuration.Generic;
using LXGaming.TorrentAnalytics.Configuration;
using LXGaming.TorrentAnalytics.Services.Web;
using NUnit.Framework;

namespace LXGaming.TorrentAnalytics.Tests.Services.Web;

public class TestWebService : WebService {

    public TestWebService(IConfiguration<Config> configuration, JsonSerializerOptions jsonSerializerOptions) : base(
        configuration, jsonSerializerOptions) {
        jsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    }

    public override async Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default) {
        var expectedElement = await base.DeserializeAsync<JsonElement>(response, cancellationToken);
        return Deserialize<T>(expectedElement);
    }

    private T Deserialize<T>(JsonElement expectedElement) {
        Assert.That(expectedElement, Is.Not.Default);

        var expectedString = JsonSerializer.Serialize(expectedElement, JsonSerializerOptions);
        Assert.That(expectedString, Is.Not.Null.And.Not.Empty);

        var actualObject = expectedElement.Deserialize<T>(JsonSerializerOptions);
        Assert.That(actualObject, Is.Not.Null);

        var actualElement = JsonSerializer.SerializeToElement<T>(actualObject!, JsonSerializerOptions);
        Assert.That(actualElement, Is.Not.Default);

        var actualString = JsonSerializer.Serialize(actualElement, JsonSerializerOptions);
        Assert.That(actualString, Is.Not.Null.And.Not.Empty);

        Warn.Unless(actualString, Is.EqualTo(expectedString).IgnoreCase);
        return actualObject!;
    }
}