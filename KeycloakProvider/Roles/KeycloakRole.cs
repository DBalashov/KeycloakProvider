namespace KeycloakProvider;

public sealed record KeycloakRole(string                     Id,
                                  string                     Name,
                                  string                     Description,
                                  bool                       Composite,
                                  bool                       ClientRole,
                                  string                     ContainerId,
                                  Dictionary<string, string> Attributes);