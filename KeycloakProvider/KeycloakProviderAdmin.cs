namespace KeycloakProvider;

public sealed class KeycloakProviderImp : IKeycloakProvider
{
    readonly KeycloakProviderConfig config;

    IKeycloakUsersProvider?      users;
    IKeycloakGroupsProvider?     groups;
    IKeycloakRolesProvider?      roles;
    IKeycloakUserGroupsProvider? userGroups;
    IKeycloakUserRolesProvider?  userRoles;

    public IKeycloakUsersProvider      Users      => users ??= new KeycloakUsersProvider(config);
    public IKeycloakGroupsProvider     Groups     => groups ??= new KeycloakGroupsProvider(config);
    public IKeycloakRolesProvider      Roles      => roles ??= new KeycloakRolesProvider(config);
    public IKeycloakUserGroupsProvider UserGroups => userGroups ??= new KeycloakUserGroupsProvider(config);
    public IKeycloakUserRolesProvider  UserRoles  => userRoles ??= new KeycloakUserRolesProvider(config, this);

    public KeycloakProviderImp(KeycloakProviderConfig config) => this.config = config;
}