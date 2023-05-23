namespace KeycloakProvider;

public interface IKeycloakRolesProvider
{
    Task<KeycloakRole[]> GetItems();

    Task<KeycloakRole?> Get(string roleName);

    Task<bool> Delete(string roleName);
    
    // Task<KeycloakGroupDetail?> GetById(string groupId);
    //
    // Task<bool> Delete(string groupId);
    //
    // Task Create(KeycloakUpdateGroup request);
    //
    // Task<bool> Update(string groupId, KeycloakUpdateGroup request);
}