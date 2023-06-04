namespace KeycloakProvider;

public interface IKeycloakClientsProvider
{
    Task<KeycloakClientRole[]> GetRoles(string clientId);

    Task<KeycloakClientScope[]> GetScopes();

    Task<KeycloakClientPermission[]> GetPermissions(string clientId);
}