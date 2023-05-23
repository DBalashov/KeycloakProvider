using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace KeycloakProvider;

abstract class BaseProviderAdmin : BaseProvider<KeycloakProviderConfig>
{
    readonly TokenStore tokenStore;
    
    protected BaseProviderAdmin(KeycloakProviderConfig config) : base(config) => 
        tokenStore = new TokenStore(c, Url, config);

    protected async Task<HttpRequestMessage> BuildMessage(string suffix, HttpMethod? method = null, object? body = null)
    {
        ArgumentNullException.ThrowIfNull(suffix);

        var token = await tokenStore.GetToken();

        var req = new HttpRequestMessage(method ?? HttpMethod.Get, $"{Url}/admin/realms/{Realm}/{suffix}")
                  {
                      Headers = {Authorization = new AuthenticationHeaderValue("Bearer", token)}
                  };

        if (body != null)
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        return req;
    }
}