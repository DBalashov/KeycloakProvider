using System.Net;
using System.Net.Http.Json;

namespace KeycloakProvider;

public class KeycloakProviderAuthImp : BaseProvider<KeycloakProviderAuthConfig>
{
    readonly TokensStore?      tokensStore;
    readonly NetworkCredential clientCredentials;

    public KeycloakProviderAuthImp(KeycloakProviderAuthConfig config, TokensStore? tokensStore = null) : base(config)
    {
        this.tokensStore  = tokensStore;
        clientCredentials = new NetworkCredential(config.ClientId, config.ClientSecret);

        var url = string.IsNullOrEmpty(config.ServerUrl)
                      ? new Uri(config.Authority).GetComponents(UriComponents.Host | UriComponents.Scheme, UriFormat.Unescaped)
                      : config.ServerUrl;
        Url = $"{url}/realms/{config.Realm}/protocol/openid-connect/token";
    }

    public async Task<TokenContainer> GetToken(string userName, string userPassword)
    {
        ArgumentNullException.ThrowIfNull(userName);
        ArgumentNullException.ThrowIfNull(userPassword);

        var req = new FormUrlEncodedContent(new Dictionary<string, string>
                                            {
                                                ["grant_type"]    = "password",
                                                ["scope"]         = "openid",
                                                ["client_id"]     = clientCredentials.UserName,
                                                ["client_secret"] = clientCredentials.Password,
                                                ["username"]      = userName,
                                                ["password"]      = userPassword
                                            });
        var resp           = await c.PostAsync(Url, req);
        var tokenContainer = await convert(resp);
        tokensStore?.AddToken(tokenContainer);
        return tokenContainer;
    }

    public async Task<TokenContainer> RefreshToken(string refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var req = new FormUrlEncodedContent(new Dictionary<string, string>
                                            {
                                                ["grant_type"]    = "refresh_token",
                                                ["scope"]         = "openid",
                                                ["client_id"]     = clientCredentials.UserName,
                                                ["client_secret"] = clientCredentials.Password,
                                                ["refresh_token"] = refreshToken
                                            });
        var resp = await c.PostAsync(Url, req);
        return await convert(resp);
    }

    public ValueTask<string?> GetToken(string accessToken)
    {
        ArgumentNullException.ThrowIfNull(accessToken);

        if (tokensStore == null) throw new NotSupportedException(Errors.TokenStoreNotConfigured);
        return tokensStore.GetToken(accessToken);
    }

    #region internals

    async Task<TokenContainer> convert(HttpResponseMessage resp)
    {
        var r = await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<InternalTokenContainer>();
        return new TokenContainer(r!.access_token,
                                  DateTime.UtcNow.AddSeconds(r.expires_in),
                                  r.refresh_token,
                                  DateTime.UtcNow.AddSeconds(r.refresh_expires_in),
                                  (r.scope ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                                  r.session_state);
    }

    public sealed record InternalTokenContainer(string access_token,
                                                string refresh_token,
                                                int    expires_in,
                                                int    refresh_expires_in,
                                                string session_state,
                                                string scope);

    /*
    {
      "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIyV1Fmd2FrTkNEOGMyWVItUzFOb3NSNVJFc3lWbHBkNzh0LUltOU1iSEpBIn0.eyJleHAiOjE2ODQ3ODMyMjksImlhdCI6MTY4NDc4MTc4OSwianRpIjoiMTUwMjEwMmItYTQ0ZS00Y2NhLWI4MzAtNzMyM2MzMzZlOGZjIiwiaXNzIjoiaHR0cHM6Ly9hdXRoLmRlbmlzaW8udGVjaC9yZWFsbXMvaGgiLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiYWJmNmYwODUtNzU2Yi00Y2I4LTljYTYtYzIzYmEzOGEwMjdhIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoid2ViLWFwaSIsInNlc3Npb25fc3RhdGUiOiJmY2Q0NzQ1NC1mOGRiLTRlYTMtOGIzMi0yOWJkNmEyOTQ3YjciLCJhY3IiOiIxIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwid2ViLXBheW1lbnRzIiwidW1hX2F1dGhvcml6YXRpb24iLCJ3ZWItYWRtaW4iLCJkZWZhdWx0LXJvbGVzLWhoIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgZW1haWwgY3VzdG9tLWF0dHJpYnV0ZXMgcHJvZmlsZSIsInNpZCI6ImZjZDQ3NDU0LWY4ZGItNGVhMy04YjMyLTI5YmQ2YTI5NDdiNyIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJuYW1lIjoiQWxleGFuZHJhIEtvdmFsZXZhIFs2MTA3XSIsInByZWZlcnJlZF91c2VybmFtZSI6ImFsZXhhbmRyYS5rb3ZhbGV2YUB5YWhvby5jb20iLCJhdmF0YXIiOiIvdXNlci1pY29ucy9tYW4ucG5nIiwiZ2l2ZW5fbmFtZSI6IkFsZXhhbmRyYSIsImZhbWlseV9uYW1lIjoiS292YWxldmEgWzYxMDddIiwiZW1haWwiOiJhbGV4YW5kcmEua292YWxldmFAeWFob28uY29tIn0.piKq92fbXAl09QLJZKY-pZ6tcCr1uQeYIHcMv9JHRUbEuBvU5We4DxZY9NxPc3nsvCZkYkkxYbUapWMFyLDzuQm2D3-STw6JwEn6ve2Mc50FX1_DEk2mLtqpv3zbxLulMEy0INU35blOteZ7fnp9ghzDEuZfI-lj5tTu50sWkAoY48Qvle3dl2oYbWcaxjwWuBWDJiSAtd3y51VNL50LDbQRfNkY9G3PLzRdZAPym7y1_IxaMMCDbkAcRmUA0zSBQssFgGc1yzH0GXa57WQOoIF6IHWhssVLgoi8Yj_8sqXSDwUg7ML2ikTQKWpkC2Lkoz0VA3S7E_rNrwwSxhWJyw",
      "expires_in": 1440,
      "refresh_expires_in": 1800,
      "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJkNTYxMzg5YS1iZGI0LTQ5MDMtYjdkMC00MDI5MzZlOWFiZmQifQ.eyJleHAiOjE2ODQ3ODM1ODksImlhdCI6MTY4NDc4MTc4OSwianRpIjoiNTA1YTQ3NWMtNzkwZi00NGJhLThiZWYtYjY1NDhiYWQ2NzJiIiwiaXNzIjoiaHR0cHM6Ly9hdXRoLmRlbmlzaW8udGVjaC9yZWFsbXMvaGgiLCJhdWQiOiJodHRwczovL2F1dGguZGVuaXNpby50ZWNoL3JlYWxtcy9oaCIsInN1YiI6ImFiZjZmMDg1LTc1NmItNGNiOC05Y2E2LWMyM2JhMzhhMDI3YSIsInR5cCI6IlJlZnJlc2giLCJhenAiOiJ3ZWItYXBpIiwic2Vzc2lvbl9zdGF0ZSI6ImZjZDQ3NDU0LWY4ZGItNGVhMy04YjMyLTI5YmQ2YTI5NDdiNyIsInNjb3BlIjoib3BlbmlkIGVtYWlsIGN1c3RvbS1hdHRyaWJ1dGVzIHByb2ZpbGUiLCJzaWQiOiJmY2Q0NzQ1NC1mOGRiLTRlYTMtOGIzMi0yOWJkNmEyOTQ3YjcifQ.pzLW8fodPqg5W5XreovieJo9wpt4W2hkeRShaAfpwWc",
      "token_type": "Bearer",
      "id_token": "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIyV1Fmd2FrTkNEOGMyWVItUzFOb3NSNVJFc3lWbHBkNzh0LUltOU1iSEpBIn0.eyJleHAiOjE2ODQ3ODMyMjksImlhdCI6MTY4NDc4MTc4OSwiYXV0aF90aW1lIjowLCJqdGkiOiJlZGUzYmM1Ni00ZGMzLTRlMTktOWJhZS0zMzIxOTY5NDNjNzEiLCJpc3MiOiJodHRwczovL2F1dGguZGVuaXNpby50ZWNoL3JlYWxtcy9oaCIsImF1ZCI6IndlYi1hcGkiLCJzdWIiOiJhYmY2ZjA4NS03NTZiLTRjYjgtOWNhNi1jMjNiYTM4YTAyN2EiLCJ0eXAiOiJJRCIsImF6cCI6IndlYi1hcGkiLCJzZXNzaW9uX3N0YXRlIjoiZmNkNDc0NTQtZjhkYi00ZWEzLThiMzItMjliZDZhMjk0N2I3IiwiYXRfaGFzaCI6IjM2MjJyUTdSQVcwUG9RejRZaG9GWUEiLCJhY3IiOiIxIiwic2lkIjoiZmNkNDc0NTQtZjhkYi00ZWEzLThiMzItMjliZDZhMjk0N2I3IiwiZW1haWxfdmVyaWZpZWQiOnRydWUsInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsIndlYi1wYXltZW50cyIsInVtYV9hdXRob3JpemF0aW9uIiwid2ViLWFkbWluIiwiZGVmYXVsdC1yb2xlcy1oaCJdfSwibmFtZSI6IkFsZXhhbmRyYSBLb3ZhbGV2YSBbNjEwN10iLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJhbGV4YW5kcmEua292YWxldmFAeWFob28uY29tIiwiYXZhdGFyIjoiL3VzZXItaWNvbnMvbWFuLnBuZyIsImdpdmVuX25hbWUiOiJBbGV4YW5kcmEiLCJmYW1pbHlfbmFtZSI6IktvdmFsZXZhIFs2MTA3XSIsImVtYWlsIjoiYWxleGFuZHJhLmtvdmFsZXZhQHlhaG9vLmNvbSJ9.V9sDKxxx_ipkG-R5PhBlA83Ul2gubKTlEbVTHqD7TcFIqK0DI-4tGgeOaCrc_8c15G8Ngfn9CEaXE7jTieCOOGZ1-5HUuVBSQr93INatvdFeohmNqJU3t46-l93owEPjnvzFUWspnGX9Y4G_kCAGDD5VbytsC3bihUbjnrW2hCc9HlxdbnjI_kFY34Mlm9uc7QqqyGu5MY1pzGpmTJ-1ag58vP6TmLX2YqnPciu9pN7Kf2-PolkxNiZT8SmjBLzYvesqoOUNK-BUTfY2GqTJ56dd-7eKjb1FwInd64B-5ZapWNMhJRc9pu1amKLsw2c0Lajgj6w8JchENi9S0qYKzA",
      "not-before-policy": 0,
      "session_state": "fcd47454-f8db-4ea3-8b32-29bd6a2947b7",
      "scope": "openid email custom-attributes profile"
    }
     */

    #endregion
}