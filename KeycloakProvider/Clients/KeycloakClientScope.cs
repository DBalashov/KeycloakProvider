namespace KeycloakProvider;

public sealed record KeycloakClientScope(string                              ID,
                                         string                              Name,
                                         string?                             Description,
                                         string                              Protocol,
                                         KeycloakClientScopeProtocolMapper[] ProtocolMappers,
                                         Dictionary<string, string>?         Attributes);

public sealed record KeycloakClientScopeProtocolMapper(string                     ID,
                                                       string                     Name,
                                                       string                     Protocol,
                                                       string                     ProtocolMapper,
                                                       Dictionary<string, string> Config);