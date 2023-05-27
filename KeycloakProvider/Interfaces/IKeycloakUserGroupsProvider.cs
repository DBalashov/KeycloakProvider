namespace KeycloakProvider;

public interface IKeycloakUserGroupsProvider
{
    Task<KeycloakUserGroup[]> GetItems(string userId);

    Task<bool> AddTo(string userId, string groupId);

    Task<bool> RemoveFrom(string userId, string groupId);
}