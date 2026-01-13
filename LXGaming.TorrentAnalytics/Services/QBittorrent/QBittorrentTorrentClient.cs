using System.Collections.Immutable;
using System.Net;
using System.Security.Authentication;
using LXGaming.Common.Utilities;
using LXGaming.TorrentAnalytics.Services.QBittorrent.Models;
using LXGaming.TorrentAnalytics.Services.Torrent.Client;
using Microsoft.Extensions.Logging;

namespace LXGaming.TorrentAnalytics.Services.QBittorrent;

public class QBittorrentTorrentClient : TorrentClientBase {

    private readonly object _lock;
    private bool _initialAuthentication;
    private volatile Task<bool> _authenticateTask;
    private Version? _version;

    public QBittorrentTorrentClient(TorrentClientOptions options, IServiceProvider serviceProvider)
        : base(options, serviceProvider) {
        _lock = new object();
        _initialAuthentication = true;
        _authenticateTask = AuthenticateInternalAsync();
    }

    public async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> func,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        bool skipAuthentication = false, CancellationToken cancellationToken = default) {
        if (skipAuthentication) {
            using var request = func();
            return await HttpClient.SendAsync(request, completionOption, cancellationToken);
        }

        var existingAuthenticateTask = _authenticateTask;
        if (await existingAuthenticateTask) {
            using var request = func();
            var response = await HttpClient.SendAsync(request, completionOption, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Forbidden) {
                return response;
            }

            response.Dispose();
        }

        if (_authenticateTask == existingAuthenticateTask) {
            lock (_lock) {
                if (_authenticateTask == existingAuthenticateTask) {
                    _authenticateTask = AuthenticateInternalAsync();
                }
            }
        }

        if (await _authenticateTask) {
            using var request = func();
            return await HttpClient.SendAsync(request, completionOption, cancellationToken);
        }

        throw new AuthenticationException("Authentication failed");
    }

    protected async Task<bool> AuthenticateInternalAsync() {
        try {
            if (!await LoginAsync()) {
                return false;
            }

            _version = await GetAppVersionAsync(true);

            Logger.LogInformation("{State} with {Client} v{Version}",
                _initialAuthentication ? "Authenticated" : "Reauthenticated", this, _version);

            _initialAuthentication = false;
            return true;
        } catch (Exception ex) {
            Logger.LogWarning(ex, "Encountered an error while authenticating with {Client}", this);
            return false;
        }
    }

    public bool IsVersionAtLeast(int major, int minor, int build) {
        if (_version == null) {
            throw new InvalidOperationException("qBittorrent version is unavailable");
        }

        return _version.Major >= major && _version.Minor >= minor && _version.Build >= build;
    }

    #region Auth
    protected async Task<bool> LoginAsync() {
        if (Options.BypassAuthentication) {
            return true;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/v2/auth/login");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string?> {
            { "username", Options.Username },
            { "password", Options.Password }
        });
        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return content switch {
            "Ok." => true,
            "Fails." => false,
            _ => throw new InvalidOperationException($"{content} is not supported")
        };
    }

    protected async Task LogoutAsync() {
        if (Options.BypassAuthentication) {
            return;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/v2/auth/logout");
        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
    }
    #endregion

    #region App
    public Task<Version> GetAppVersionAsync() {
        return GetAppVersionAsync(false);
    }

    protected async Task<Version> GetAppVersionAsync(bool skipAuthentication) {
        using var response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, "api/v2/app/version"),
            HttpCompletionOption.ResponseHeadersRead, skipAuthentication);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return StringUtils.ParseVersion(content);
    }

    public Task<Version> GetAppWebApiVersionAsync() {
        return GetAppWebApiVersionAsync(false);
    }

    protected async Task<Version> GetAppWebApiVersionAsync(bool skipAuthentication) {
        using var response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, "api/v2/app/webapiVersion"),
            HttpCompletionOption.ResponseHeadersRead, skipAuthentication);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return StringUtils.ParseVersion(content);
    }

    public async Task<AppBuildInfo> GetAppBuildInfoAsync() {
        using var response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, "api/v2/app/buildInfo"),
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await WebService.DeserializeAsync<AppBuildInfo>(response);
    }
    #endregion

    #region Torrents
    public async Task<ImmutableArray<TorrentInfo>> GetTorrentInfosAsync(TorrentFilter? filter = null,
        string? category = null, string? tag = null, string? sort = null, bool? reverse = null, int? limit = null,
        int? offset = null, IEnumerable<string>? hashes = null, bool? isPrivate = null, bool? includeTrackers = null) {
        string? filterString;
        if (filter != null) {
            if (IsVersionAtLeast(5, 0, 0)) {
                filterString = StringUtils.GetEnumName(filter switch {
                    TorrentFilter.Resumed => TorrentFilter.Running,
                    TorrentFilter.Paused => TorrentFilter.Stopped,
                    _ => filter!
                });
            } else {
                filterString = StringUtils.GetEnumName(filter switch {
                    TorrentFilter.Running => TorrentFilter.Resumed,
                    TorrentFilter.Stopped => TorrentFilter.Paused,
                    _ => filter!
                });
            }
        } else {
            filterString = null;
        }

        var queries = new Dictionary<string, string?>();
        CollectionUtils.AddIgnoreNull(queries, "filter", filterString);
        CollectionUtils.AddIgnoreNull(queries, "category", category);
        CollectionUtils.AddIgnoreNull(queries, "tag", tag);
        CollectionUtils.AddIgnoreNull(queries, "sort", sort);
        CollectionUtils.AddIgnoreNull(queries, "reverse", reverse?.ToString());
        CollectionUtils.AddIgnoreNull(queries, "limit", limit?.ToString());
        CollectionUtils.AddIgnoreNull(queries, "offset", offset?.ToString());
        CollectionUtils.AddIgnoreNull(queries, "hashes", hashes != null ? string.Join('|', hashes) : null);
        CollectionUtils.AddIgnoreNull(queries, "private", isPrivate?.ToString());
        CollectionUtils.AddIgnoreNull(queries, "includeTrackers", includeTrackers?.ToString());
        var queryString = HttpUtils.CreateQueryString(queries);

        using var response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, $"api/v2/torrents/info{queryString}"),
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await WebService.DeserializeAsync<ImmutableArray<TorrentInfo>>(response);
    }

    public async Task<ImmutableArray<TorrentTracker>> GetTorrentTrackersAsync(string hash) {
        var queryString = HttpUtils.CreateQueryString("hash", hash);

        using var response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, $"api/v2/torrents/trackers{queryString}"),
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await WebService.DeserializeAsync<ImmutableArray<TorrentTracker>>(response);
    }
    #endregion
}