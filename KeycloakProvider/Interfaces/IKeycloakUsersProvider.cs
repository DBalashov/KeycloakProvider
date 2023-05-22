namespace KeycloakProvider;

public interface IKeycloakUsersProvider
{
    Task<KeycloakUser?> GetById(string userId);

    Task<KeycloakUser[]> Find(bool exact, KeycloakFindUser parms);

    Task Create(KeycloakCreateUser request);

    Task<bool> Delete(string userId);

    Task<bool> Update(string userId, KeycloakUpdateUser request);

    Task<bool> ChangeState(string userId, bool newState);

    Task<bool> UpdateAttributes(string userId, Dictionary<string, string?> attributes);

    Task<string[]> GetDisabled(params string[] userIds);

    Task<KeycloakUserGroup[]> GetGroups(string userId);

    Task ResetPassword(string userId, string password);

    Task Logout(string userId);

    Task<KeycloakUserSession[]> GetSessions(string userId);
}