namespace KeycloakProvider;

public interface IKeycloakRolesProvider
{
    Task<KeycloakRole[]> GetItems();

    Task<KeycloakRole?> Get(string     roleName);
    Task<KeycloakRole?> GetById(string roleId);

    Task Create(KeycloakCreateRole request);

    Task<bool> Update(string roleId, KeycloakUpdateRole request);

    Task<bool> Delete(string roleId);
}