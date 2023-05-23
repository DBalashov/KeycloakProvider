using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace KeycloakProvider;

abstract class BaseProviderAdmin : BaseProvider<KeycloakProviderConfig>
{
    readonly TokenStore tokenStore;

    protected BaseProviderAdmin(KeycloakProviderConfig config) : base(config) =>
        tokenStore = new TokenStore(c, Url, config);

    protected async Task<HttpRequestMessage> BuildMessage(string suffix, HttpMethod? method = null, KeycloakRequest? body = null)
    {
        ArgumentNullException.ThrowIfNull(suffix);

        var token = await tokenStore.GetToken();

        var req = new HttpRequestMessage(method ?? HttpMethod.Get, $"{Url}/admin/realms/{Realm}/{suffix}")
                  {
                      Headers = {Authorization = new AuthenticationHeaderValue("Bearer", token)}
                  };

        if (body != null)
            req.Content = body.AsHttpContent();

        return req;
    }

    #region SendAndGetResponse<T> / SendWithoutResponse

    protected async Task<T?> SendAndGetResponse<T>(HttpRequestMessage req) where T : class
    {
        var resp = await c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;

        return await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<T>();
    }

    protected async Task<bool> SendWithoutResponse(HttpRequestMessage req, bool falseIfNotFound = true)
    {
        var resp = await c.SendAsync(req);
        if (falseIfNotFound)
        {
            if (resp.StatusCode == HttpStatusCode.NotFound) return false;
        }

        resp.EnsureSuccessStatusCode();
        return true;
    }

    #endregion
}