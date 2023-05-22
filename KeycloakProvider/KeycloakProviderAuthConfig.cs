namespace KeycloakProvider;

/// <param name="Authority">url to server, including schema and ending by /realms/{realm} suffix, like "https://serverName.zone/realms/someRealm"</param>
/// <param name="ClientId">client id in realm with 'Direct Access Grants' in 'Authentication flow' enabled AND 'Client authentication' enabled</param>
/// /// <param name="ServerUrl">if empty - take server name from Authority</param>
/// <param name="RequestTimeout">3 seconds if not specified</param>
public record KeycloakProviderAuthConfig(string   Realm,    string Authority,
                                         string   ClientId, string ClientSecret,
                                         string?  ServerUrl      = null,
                                         TimeSpan RequestTimeout = default);