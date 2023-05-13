namespace KeycloakProvider;

sealed class ProviderSettings
{
    internal readonly string     Url;
    internal readonly string     Realm;
    internal readonly HttpClient c;
    internal readonly TokenStore TokenStore;

    public ProviderSettings(KeycloakProviderConfig config)
    {
        c         = new HttpClient();
        c.Timeout = config.RequestTimeout == TimeSpan.Zero ? TimeSpan.FromSeconds(3) : config.RequestTimeout;

        Url = string.IsNullOrEmpty(config.ServerUrl)
                  ? new Uri(config.Authority).GetComponents(UriComponents.Host | UriComponents.Scheme, UriFormat.Unescaped)
                  : config.ServerUrl;

        TokenStore = new TokenStore(c, Url, config);
        Realm      = config.Realm;
    }
}