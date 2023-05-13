using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace KeycloakProvider;

abstract class BaseProvider
{
    internal readonly ProviderSettings settings;
    
    protected BaseProvider(ProviderSettings settings) => this.settings = settings;

    protected async Task<HttpRequestMessage> BuildMessage(string suffix, HttpMethod? method = null, object? body = null)
    {
        var token = await settings.TokenStore.GetToken();

        var req = new HttpRequestMessage(method ?? HttpMethod.Get, $"{settings.Url}/admin/realms/{settings.Realm}/{suffix}");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (body != null)
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        return req;
    }
}