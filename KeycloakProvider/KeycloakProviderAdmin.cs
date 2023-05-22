namespace KeycloakProvider;

public sealed class KeycloakProviderImp : IKeycloakProvider
{
    readonly KeycloakProviderConfig config;

    IKeycloakUsersProvider?  users;
    IKeycloakGroupsProvider? groups;
    IKeycloakRolesProvider?  roles;

    public IKeycloakUsersProvider  Users  => users ??= new KeycloakUsersProvider(config);
    public IKeycloakGroupsProvider Groups => groups ??= new KeycloakGroupsProvider(config);
    public IKeycloakRolesProvider  Roles  => roles ??= new KeycloakRolesProvider(config);

    public KeycloakProviderImp(KeycloakProviderConfig config) => this.config = config;
}