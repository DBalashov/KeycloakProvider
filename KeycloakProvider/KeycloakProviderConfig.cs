namespace KeycloakProvider;

public record KeycloakProviderConfig(string   Realm,         string Authority,
                                     string   ClientId,      string ClientSecret,
                                     string   AdminClientId, string AdminClientSecret,
                                     string?  ServerUrl      = null,
                                     TimeSpan RequestTimeout = default);