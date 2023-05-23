﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace KeycloakProvider;

abstract class BaseProvider
{
    readonly TokenStore tokenStore;

    protected readonly string Url;
    protected readonly string Realm;

    protected readonly HttpClient c = new();

    protected BaseProvider(KeycloakProviderConfig config)
    {
        c.Timeout = config.RequestTimeout == TimeSpan.Zero ? TimeSpan.FromSeconds(3) : config.RequestTimeout;

        Url = string.IsNullOrEmpty(config.ServerUrl)
                  ? new Uri(config.Authority).GetComponents(UriComponents.Host | UriComponents.Scheme, UriFormat.Unescaped)
                  : config.ServerUrl;

        tokenStore = new TokenStore(c, Url, config);
        Realm      = config.Realm;
    }

    protected async Task<HttpRequestMessage> BuildMessage(string suffix, HttpMethod? method = null, object? body = null)
    {
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