namespace KeycloakProvider;

public sealed record UserRoleItem(string  ID,
                                  string  Name,
                                  string? Description,
                                  bool    ClientRole,
                                  bool    Composite,
                                  string  ContainerId);