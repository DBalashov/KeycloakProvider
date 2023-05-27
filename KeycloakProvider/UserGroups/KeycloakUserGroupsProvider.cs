namespace KeycloakProvider;

sealed class KeycloakUserGroupsProvider : BaseProviderAdmin, IKeycloakUserGroupsProvider
{
    public KeycloakUserGroupsProvider(KeycloakProviderConfig config) : base(config)
    {
    }

    public async Task<KeycloakUserGroup[]> GetItems(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req    = await BuildMessage($"users/{userId}/groups", HttpMethod.Get);
        var groups = await SendAndGetResponse<KeycloakUserGroup[]>(req);
        return groups ?? Array.Empty<KeycloakUserGroup>();
    }

    public async Task<bool> AddTo(string userId, string groupId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req = await BuildMessage($"users/{userId}/groups/{groupId}", HttpMethod.Put);
        return await SendWithoutResponse(req);
    }

    public async Task<bool> RemoveFrom(string userId, string groupId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req = await BuildMessage($"users/{userId}/groups/{groupId}", HttpMethod.Delete);
        return await SendWithoutResponse(req);
    }
}