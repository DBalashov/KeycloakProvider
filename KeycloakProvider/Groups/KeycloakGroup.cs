using System.Diagnostics.CodeAnalysis;

namespace KeycloakProvider;

[ExcludeFromCodeCoverage]
public sealed record KeycloakGroup(string ID, string Name, string Path, KeycloakGroup[] Subgroups);

[ExcludeFromCodeCoverage]
public sealed record KeycloakGroupDetail(string                     ID,
                                         string                     Name,
                                         string                     Path,
                                         string[]                   RealmRoles,
                                         KeycloakGroupAccess        Access,
                                         Dictionary<string, string> Attributes,
                                         KeycloakGroupDetail[]      Subgroups);

[ExcludeFromCodeCoverage]
public sealed record KeycloakGroupAccess(bool View,
                                         bool Manage,
                                         bool ManageMembership);