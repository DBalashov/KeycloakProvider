namespace KeycloakProvider;

public interface IKeycloakGroupsProvider
{
    Task<KeycloakGroup[]> GetItems();

    Task<KeycloakGroupDetail?> GetById(string groupId);

    Task<bool> Delete(string groupId);

    Task Create(KeycloakUpdateGroup request);

    Task<bool> Update(string groupId, KeycloakUpdateGroup request);
}