namespace KeycloakProvider;

public abstract class BaseProvider<T> where T : KeycloakProviderAuthConfig
{
    protected string Url;

    protected readonly string     Realm;
    protected readonly HttpClient c = new();

    protected BaseProvider(T config)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(config.Realm);
        ArgumentNullException.ThrowIfNull(config.Authority);

        c.Timeout = config.RequestTimeout == TimeSpan.Zero ? TimeSpan.FromSeconds(3) : config.RequestTimeout;

        Url = string.IsNullOrEmpty(config.ServerUrl)
                  ? new Uri(config.Authority).GetComponents(UriComponents.Host | UriComponents.Scheme, UriFormat.Unescaped)
                  : config.ServerUrl;

        Realm = config.Realm;
    }
}