using System.Diagnostics.CodeAnalysis;

namespace KeycloakProvider;

[ExcludeFromCodeCoverage]
public sealed class KeycloakProviderImp : IKeycloakProvider
{
    readonly KeycloakProviderConfig config;
    readonly HttpClient             c;

    IKeycloakUsersProvider?      users;
    IKeycloakGroupsProvider?     groups;
    IKeycloakRolesProvider?      roles;
    IKeycloakUserGroupsProvider? userGroups;
    IKeycloakUserRolesProvider?  userRoles;

    public IKeycloakUsersProvider      Users      => users ??= new KeycloakUsersProvider(config, c);
    public IKeycloakGroupsProvider     Groups     => groups ??= new KeycloakGroupsProvider(config, c);
    public IKeycloakRolesProvider      Roles      => roles ??= new KeycloakRolesProvider(config, c);
    public IKeycloakUserGroupsProvider UserGroups => userGroups ??= new KeycloakUserGroupsProvider(config, c);
    public IKeycloakUserRolesProvider  UserRoles  => userRoles ??= new KeycloakUserRolesProvider(config, Roles, c);

    public KeycloakProviderImp(KeycloakProviderConfig config, HttpClient? c = null)
    {
        this.config = config;
        this.c      = c ?? new HttpClient();
    }
}