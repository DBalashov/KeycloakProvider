namespace KeycloakProvider;

public interface IKeycloakRolesProvider
{
    Task<KeycloakRole[]> GetItems();

    Task<KeycloakRole?> Get(string roleName);

    Task<bool> Delete(string roleId);

    Task<bool> UpdateAttributes(string roleId, Dictionary<string, string?> attributes);

    Task<KeycloakRole?> GetById(string roleId);

    Task Create(KeycloakCreateRole request);

    Task<bool> Update(string roleId, KeycloakUpdateRole request);
}