namespace KeycloakProvider;

public interface IKeycloakGroupsProvider
{
    Task<KeycloakGroup[]> GetItems();

    Task<KeycloakGroupDetail?> GetById(string groupId);

    Task<bool> Delete(string groupId);

    Task Create(KeycloakCreateRole request);

    Task<bool> Update(string groupId, KeycloakUpdateGroup request);

    Task<bool> UpdateAttributes(string groupId, Dictionary<string, string?> attributes);
}