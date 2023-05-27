using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace KeycloakProvider;

/// <summary>
/// Store refresh token as dictionary[hash_of_access_token : refresh_token] in memory and automatically refresh them if needed
/// (while GetToken is called)
/// Access token NOT STORED, only hash 
/// </summary>
public sealed class TokensStore
{
    IKeycloakProviderAuth authProvider;

    readonly IMemoryCache  cache     = new MemoryCache(new MemoryCacheOptions());
    readonly SemaphoreSlim semaphore = new(1, 1);

    public TokensStore(IKeycloakProviderAuth authProvider) => this.authProvider = authProvider;

    internal void AddToken(TokenContainer tc) =>
        cache.Set(hashString(tc.AccessToken), new RefreshTokenContainer(tc.AccessTokenExpired, tc.RefreshToken), tc.RefreshTokenExpired);

    public async ValueTask<string?> GetToken(string accessToken)
    {
        ArgumentNullException.ThrowIfNull(accessToken);
        var accessTokenKey = hashString(accessToken);

        var rtc = cache.Get<RefreshTokenContainer>(accessTokenKey);
        if (rtc == null)
        {
            Debug.WriteLine("RefreshToken not found in cache", "TokensStore");
            return null;
        }

        // no refresh required, access token valid
        if (rtc.AccessTokenExpired > DateTime.UtcNow)
        {
            Debug.WriteLine($"Access token valid (expiration: {rtc.AccessTokenExpired:s}), return", "TokensStore");
            return accessToken;
        }

        // need to refresh access token
        await semaphore.WaitAsync();
        try
        {
            Debug.WriteLine("Refresh access token", "TokensStore");
            var r = await authProvider.RefreshToken(rtc.RefreshToken);

            Debug.WriteLine("Replace token in cache", "TokensStore");
            cache.Remove(accessTokenKey);
            cache.Set(hashString(r.AccessToken), new RefreshTokenContainer(r.AccessTokenExpired, r.RefreshToken), r.RefreshTokenExpired);
            return r.AccessToken;
        }
        finally
        {
            semaphore.Release();
        }
    }

    string hashString(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return Convert.ToBase64String(SHA256.HashData(Encoding.Default.GetBytes(source)));
    }

    sealed record RefreshTokenContainer(DateTime AccessTokenExpired, string RefreshToken);
}