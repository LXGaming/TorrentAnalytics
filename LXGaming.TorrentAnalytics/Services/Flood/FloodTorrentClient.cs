using System.Net;
using System.Security.Authentication;
using LXGaming.TorrentAnalytics.Services.Flood.Models;
using LXGaming.TorrentAnalytics.Services.Torrent.Client;
using Microsoft.Extensions.Logging;

namespace LXGaming.TorrentAnalytics.Services.Flood;

public class FloodTorrentClient : TorrentClientBase {

    private readonly object _lock;
    private bool _initialAuthentication;
    private volatile Task<bool> _authenticateTask;

    public FloodTorrentClient(TorrentClientOptions options, IServiceProvider serviceProvider)
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
            var authenticate = await AuthenticateAsync();
            if (!authenticate.Success) {
                return false;
            }

            Logger.LogInformation("{State} with {Client} as {Username} ({Level})",
                _initialAuthentication ? "Authenticated" : "Reauthenticated", this, authenticate.Username,
                authenticate.Level);

            _initialAuthentication = false;
            return true;
        } catch (Exception ex) {
            Logger.LogWarning(ex, "Encountered an error while authenticating with {Client}", this);
            return false;
        }
    }

    #region Auth
    protected async Task<Authenticate> AuthenticateAsync() {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/authenticate");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string?> {
            { "username", Options.Username },
            { "password", Options.Password }
        });
        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await WebService.DeserializeAsync<Authenticate>(response);
    }
    #endregion

    #region Torrents
    public async Task<TorrentListSummary> GetTorrentsAsync() {
        using var response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, "api/torrents"),
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await WebService.DeserializeAsync<TorrentListSummary>(response);
    }
    #endregion
}