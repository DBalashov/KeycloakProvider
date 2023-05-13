namespace KeycloakProvider;

public interface IKeycloakProvider
{
    IKeycloakUsersProvider Users { get; }

    IKeycloakGroupsProvider Groups { get; }
}

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

public interface IKeycloakGroupsProvider
{
    Task<KeycloakGroup[]> GetItems();

    Task<KeycloakGroupDetail?> GetById(string groupId);

    Task<bool> Delete(string groupId);

    Task Create(KeycloakUpdateGroup request);

    Task<bool> Update(string groupId, KeycloakUpdateGroup request);
}

public interface IKeycloakRolesProvider
{
    Task<KeycloakRole[]> GetItems();

    Task<KeycloakRole?> Get(string roleName);

    Task Delete(string roleName);
    //
    // Task<KeycloakGroupDetail?> GetById(string groupId);
    //
    // Task<bool> Delete(string groupId);
    //
    // Task Create(KeycloakUpdateGroup request);
    //
    // Task<bool> Update(string groupId, KeycloakUpdateGroup request);
}