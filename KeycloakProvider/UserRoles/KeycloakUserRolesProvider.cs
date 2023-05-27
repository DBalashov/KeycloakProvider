namespace KeycloakProvider;

sealed class KeycloakUserRolesProvider : BaseProviderAdmin, IKeycloakUserRolesProvider
{
    readonly IKeycloakRolesProvider rolesProvider;

    public KeycloakUserRolesProvider(KeycloakProviderConfig config, IKeycloakRolesProvider rolesProvider, HttpClient c) : base(config, c) => this.rolesProvider = rolesProvider;

    public async Task<UserRoleItem[]> GetItems(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req   = await BuildMessage($"users/{userId}/role-mappings", HttpMethod.Get);
        var items = (await SendAndGetResponse<UserRoleMappingResponse>(req))?.realmMapping;
        return (items ?? Array.Empty<UserRoleMapping>()).Select(p => new UserRoleItem(p.id, p.name, p.description, p.clientRole, p.composite, p.containerId)).ToArray();
    }

    public async Task<bool> Assign(string userId, params string[] roleIds)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleIds);
        if (!roleIds.Any()) throw new ArgumentException(Errors.RequestEmpty);

        var roles = await filterRoles(roleIds);
        if (!roles.Any()) return false;

        var req = await BuildMessage($"users/{userId}/role-mappings/realm", HttpMethod.Post, new KeycloakUserRoleAssign().AddRoles(roles));
        return await SendWithoutResponse(req);
    }

    public async Task<bool> Unassign(string userId, params string[] roleIds)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var roles = await filterRoles(roleIds);
        if (!roles.Any()) return false;

        var req = await BuildMessage($"users/{userId}/role-mappings/realm", HttpMethod.Delete, new KeycloakUserRoleUnassign().AddRoles(roles));
        return await SendWithoutResponse(req);
    }

    #region internals

    async Task<KeycloakRole[]> filterRoles(string[] roleIds)
    {
        ArgumentNullException.ThrowIfNull(roleIds);
        if (!roleIds.Any()) throw new ArgumentException(Errors.RequestEmpty);

        var roles = await rolesProvider.GetItems();
        return roles.Where(p => roleIds.Contains(p.ID)).ToArray();
    }

    sealed record UserRoleMappingResponse(UserRoleMapping[] realmMapping);

    sealed record UserRoleMapping(bool    clientRole,
                                  bool    composite,
                                  string  containerId,
                                  string  id,
                                  string  name,
                                  string? description);

    #endregion
}