namespace KeycloakProvider;

sealed class KeycloakRolesProvider : BaseProviderAdmin, IKeycloakRolesProvider
{
    public KeycloakRolesProvider(KeycloakProviderConfig config) : base(config)
    {
    }

    public async Task<KeycloakRole[]> GetItems()
    {
        var req   = await BuildMessage("roles?briefRepresentation=false");
        var roles = await SendAndGetResponse<KeycloakRoleInternal[]>(req);
        return (roles ?? Array.Empty<KeycloakRoleInternal>())
              .Select(p => new KeycloakRole(p.Id, p.Name, p.Description, p.Composite, p.ClientRole, p.ContainerId,
                                            p.Attributes.ToDictionary(a => a.Key, a => a.Value.First())))
              .ToArray();
    }

    public async Task<KeycloakRole?> Get(string roleName)
    {
        ArgumentNullException.ThrowIfNull(roleName);

        var req  = await BuildMessage($"roles/{roleName}");
        var role = await SendAndGetResponse<KeycloakRoleInternal>(req);
        return role == null
                   ? null
                   : new KeycloakRole(role.Id, role.Name, role.Description, role.Composite, role.ClientRole, role.ContainerId,
                                      role.Attributes.ToDictionary(a => a.Key, a => a.Value.First()));
    }

    public async Task<KeycloakRole?> GetById(string roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId);

        var req  = await BuildMessage($"roles-by-id/{roleId}");
        var role = await SendAndGetResponse<KeycloakRoleInternal>(req);
        return role == null
                   ? null
                   : new KeycloakRole(role.Id, role.Name, role.Description, role.Composite, role.ClientRole, role.ContainerId,
                                      role.Attributes.ToDictionary(a => a.Key, a => a.Value.First()));
    }

    public async Task<bool> Delete(string roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId);

        var req = await BuildMessage($"roles-by-id/{roleId}", HttpMethod.Delete);
        return await SendWithoutResponse(req);
    }

    public async Task Create(KeycloakCreateRole request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!request.Values.Any()) throw new ArgumentException(Errors.RequestEmpty);

        var req = await BuildMessage("roles", HttpMethod.Post, request);
        await SendWithoutResponse(req, false);
    }

    public async Task<bool> Update(string roleId, KeycloakUpdateRole request)
    {
        ArgumentNullException.ThrowIfNull(roleId);
        ArgumentNullException.ThrowIfNull(request);
        if (!request.Values.Any()) throw new ArgumentException(Errors.RequestEmpty);
        
        if (request.Values["attributes"] is Dictionary<string, string[]?> attrs)
        {
            var role = await GetById(roleId);
            if (role == null) return false;

            attrs.MergeExistingAttributes(role.Attributes);
        }
        
        var req = await BuildMessage($"roles-by-id/{roleId}", HttpMethod.Put, request);
        return await SendWithoutResponse(req);
    }

    #region internals

    public sealed record KeycloakRoleInternal(string                       Id,
                                              string                       Name,
                                              string                       Description,
                                              bool                         Composite,
                                              bool                         ClientRole,
                                              string                       ContainerId,
                                              Dictionary<string, string[]> Attributes);

    #endregion
}