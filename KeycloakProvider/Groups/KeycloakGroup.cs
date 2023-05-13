namespace KeycloakProvider;

public record KeycloakGroup(string ID, string Name, string Path, KeycloakGroup[] Subgroups);

public sealed record KeycloakGroupDetail(string                     ID,
                                         string                     Name,
                                         string                     Path,
                                         string[]                   RealmRoles,
                                         KeycloakGroupAccess        Access,
                                         Dictionary<string, string> Attributes,
                                         KeycloakGroupDetail[]      Subgroups);

public sealed record KeycloakGroupAccess(bool View,
                                         bool Manage,
                                         bool ManageMembership);