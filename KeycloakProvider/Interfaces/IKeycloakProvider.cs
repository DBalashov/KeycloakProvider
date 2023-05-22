namespace KeycloakProvider;

public interface IKeycloakProvider
{
    IKeycloakUsersProvider Users { get; }

    IKeycloakGroupsProvider Groups { get; }

    IKeycloakRolesProvider Roles { get; }
}