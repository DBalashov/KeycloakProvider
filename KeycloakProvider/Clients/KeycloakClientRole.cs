using System.Diagnostics.CodeAnalysis;

namespace KeycloakProvider;

[ExcludeFromCodeCoverage]
public sealed record KeycloakClientRole(string  ID,
                                        string  Name,
                                        string? Description,
                                        bool    ClientRole,
                                        bool    Composite,
                                        string  ContainerId);