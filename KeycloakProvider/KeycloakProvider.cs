namespace KeycloakProvider;

public sealed class KeycloakProviderImp : IKeycloakProvider
{
    readonly ProviderSettings settings;

    IKeycloakUsersProvider?  users;
    IKeycloakGroupsProvider? groups;
    IKeycloakRolesProvider?  roles;

    public IKeycloakUsersProvider Users => users ??= new KeycloakUsersProvider(settings);
    public IKeycloakGroupsProvider Groups => groups ??= new KeycloakGroupsProvider(settings);
    public IKeycloakRolesProvider Roles => roles ??= new KeycloakRolesProvider(settings);

    public KeycloakProviderImp(KeycloakProviderConfig config) => 
        settings = new ProviderSettings(config);
}