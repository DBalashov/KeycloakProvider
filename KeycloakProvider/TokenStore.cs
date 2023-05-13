using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace KeycloakProvider;

sealed class TokenStore
{
    readonly NetworkCredential credentials;
    readonly string            adminClientCredentials;
    readonly string            url;
    readonly HttpClient        c;
    readonly SemaphoreSlim     semaphoreSlim = new(1, 1);

    TokenContainer? tokenContainer;

    public TokenStore(HttpClient c, string url, KeycloakProviderConfig config)
    {
        this.url               = url;
        this.c                 = c;
        adminClientCredentials = Convert.ToBase64String(Encoding.Default.GetBytes($"{config.AdminClientId}:{config.AdminClientSecret}"));
        credentials            = new NetworkCredential(config.ClientId, config.ClientSecret);
    }

    public async ValueTask<string> GetToken()
    {
        // https://dl.dropboxusercontent.com/s/ckxcj6z4w87su9a/202304_211838_chrome.png
        // https://dl.dropboxusercontent.com/s/urnr4mysj5ri7uq/202304_214512_chrome.png
        await semaphoreSlim.WaitAsync();
        if (tokenContainer != null && tokenContainer.Expired > DateTime.UtcNow)
        {
            semaphoreSlim.Release();
            var remainSeconds = tokenContainer.Expired.Subtract(DateTime.UtcNow).TotalSeconds.ToString("F2", CultureInfo.InvariantCulture);
            Debug.WriteLine($"Cached token: {remainSeconds} remains", "TokenStore");
            return tokenContainer.AccessToken;
        }

        try
        {
            var oldTokenContainer = tokenContainer;
            tokenContainer = null;

            if (oldTokenContainer?.RefreshToken != null)
                tokenContainer = await refreshToken(oldTokenContainer.RefreshToken);

            tokenContainer ??= await requestToken();
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return tokenContainer.AccessToken;
    }

    #region requestToken / refreshToken

    async Task<TokenContainer> requestToken()
    {
        Debug.WriteLine("Request token", "TokenStore");
        var req = new HttpRequestMessage(HttpMethod.Post, $"{url}/realms/master/protocol/openid-connect/token");
        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", adminClientCredentials);
        req.Content = new StringContent($"username={credentials.UserName}&" +
                                        $"password={credentials.Password}&" +
                                        $"grant_type=password",
                                        Encoding.UTF8,
                                        "application/x-www-form-urlencoded");

        var resp = await c.SendAsync(req);
        var tc   = (await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<TokenContainer>())!;
        tc = tc with {Expired = DateTime.UtcNow.AddSeconds(tc.ExpiresIn - 3)};
        Debug.WriteLine($"Request token: {tc.Expired}", "TokenStore");
        return tc;
    }

    async Task<TokenContainer?> refreshToken(string refreshToken)
    {
        Debug.WriteLine("Refresh token", "TokenStore");
        var req = new HttpRequestMessage(HttpMethod.Post, $"{url}/realms/master/protocol/openid-connect/token");
        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", adminClientCredentials);
        req.Content = new StringContent($"username={credentials.UserName}&" +
                                        $"password={credentials.Password}&" +
                                        $"grant_type=refresh_token&"        +
                                        $"refresh_token={refreshToken}",
                                        Encoding.UTF8,
                                        "application/x-www-form-urlencoded");

        var resp = await c.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            Debug.WriteLine($"Can't refresh token: {resp.StatusCode}", "TokenStore");
            return null;
        }

        var tc = (await resp.Content.ReadFromJsonAsync<TokenContainer>())!;
        tc = tc with {Expired = DateTime.UtcNow.AddSeconds(tc.ExpiresIn - 3)};
        Debug.WriteLine($"Refreshed token: {tc.Expired}", "TokenStore");
        return tc;
    }

    #endregion

    sealed record TokenContainer([property: JsonPropertyName("access_token")]
                                 string AccessToken,
                                 [property: JsonPropertyName("refresh_token")]
                                 string RefreshToken,
                                 [property: JsonPropertyName("expires_in")]
                                 int ExpiresIn,
                                 DateTime Expired);
}