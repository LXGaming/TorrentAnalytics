using System.Net;
using System.Text.Json;
using LXGaming.Configuration.Generic;
using LXGaming.TorrentAnalytics.Configuration;
using LXGaming.TorrentAnalytics.Utilities;

namespace LXGaming.TorrentAnalytics.Services.Web;

public class WebService(IConfiguration<Config> configuration, JsonSerializerOptions jsonSerializerOptions) {

    public JsonSerializerOptions JsonSerializerOptions { get; } = jsonSerializerOptions;

    public virtual HttpClient CreateClient(HttpMessageHandler handler) {
        var category = configuration.Value?.WebCategory;
        if (category == null) {
            throw new InvalidOperationException("WebCategory is unavailable");
        }

        var client = new HttpClient(handler);
        try {
            client.DefaultRequestHeaders.Add("User-Agent", Constants.Application.UserAgent);
            client.Timeout = TimeSpan.FromMilliseconds(category.Timeout);
        } catch (Exception) {
            client.Dispose();
            throw;
        }

        return client;
    }

    public SocketsHttpHandler CreateHandler() {
        var category = configuration.Value?.WebCategory;
        if (category == null) {
            throw new InvalidOperationException("WebCategory is unavailable");
        }

        var handler = new SocketsHttpHandler();
        try {
            handler.AutomaticDecompression = DecompressionMethods.All;
            handler.PooledConnectionLifetime = TimeSpan.FromMilliseconds(category.PooledConnectionLifetime);
            handler.UseCookies = false;
        } catch (Exception) {
            handler.Dispose();
            throw;
        }

        return handler;
    }

    public virtual async Task<T> DeserializeAsync<T>(HttpResponseMessage response,
        CancellationToken cancellationToken = default) {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions, cancellationToken)
               ?? throw new JsonException($"Failed to deserialize {typeof(T).Name}");
    }
}