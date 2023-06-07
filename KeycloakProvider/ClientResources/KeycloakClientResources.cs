namespace KeycloakProvider;

public sealed record KeycloakClientResource(string                        ID,
                                            string                        Name,
                                            string?                       Type,
                                            string[]                      URIs,
                                            bool                          OwnerManagedAccess,
                                            string?                       DisplayName,
                                            KeycloakClientResourceScope[] Scopes,
                                            KeycloakClientResourceOwner   Owner,
                                            Dictionary<string, string>    Attributes);

public sealed record KeycloakClientResourceScope(string ID, string Name);

public sealed record KeycloakClientResourceOwner(string ID, string Name);