namespace KeycloakProvider;

sealed class KeycloakGroupsProvider : BaseProviderAdmin, IKeycloakGroupsProvider
{
    public KeycloakGroupsProvider(KeycloakProviderConfig config) : base(config)
    {
    }

    public async Task<KeycloakGroup[]> GetItems()
    {
        var req    = await BuildMessage("groups");
        var groups = await SendAndGetResponse<KeycloakGroup[]>(req);

        return groups ?? Array.Empty<KeycloakGroup>();
    }

    public async Task<KeycloakGroupDetail?> GetById(string groupId)
    {
        ArgumentNullException.ThrowIfNull(groupId);

        var req   = await BuildMessage($"groups/{groupId}");
        var group = await SendAndGetResponse<KeycloakGroupDetailInternal>(req);

        return group != null ? convert(group) : null;
    }

    public async Task<bool> Delete(string groupId)
    {
        ArgumentNullException.ThrowIfNull(groupId);

        var req = await BuildMessage($"groups/{groupId}", HttpMethod.Delete);
        return await SendWithoutResponse(req);
    }

    public async Task Create(KeycloakCreateGroup request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!request.Values.Any()) throw new ArgumentException(Errors.RequestEmpty);

        var req = await BuildMessage("groups", HttpMethod.Post, request);
        await SendWithoutResponse(req, false);
    }

    public async Task<bool> Update(string groupId, KeycloakUpdateGroup request)
    {
        ArgumentNullException.ThrowIfNull(groupId);
        ArgumentNullException.ThrowIfNull(request);
        if (!request.Values.Any()) throw new ArgumentException(Errors.RequestEmpty);

        var req = await BuildMessage($"groups/{groupId}", HttpMethod.Put, request);
        return await SendWithoutResponse(req);
    }

    public async Task<bool> UpdateAttributes(string groupId, Dictionary<string, string?> attributes)
    {
        ArgumentNullException.ThrowIfNull(groupId);
        ArgumentNullException.ThrowIfNull(attributes);

        var group = await GetById(groupId);
        if (group == null) return false;

        var newAttributes = group.Attributes.MergeAttributes(attributes);
        var req           = await BuildMessage($"groups/{groupId}", HttpMethod.Put, new KeycloakUpdateAttribute(newAttributes));
        return await SendWithoutResponse(req);
    }

    #region internals

    KeycloakGroupDetail convert(KeycloakGroupDetailInternal from) =>
        new(from.ID, from.Name, from.Path, from.RealmRoles, from.Access,
            from.Attributes.ToDictionary(p => p.Key, p => p.Value[0]),
            from.Subgroups.Select(convert).ToArray());

    sealed record KeycloakGroupDetailInternal(string                        ID,
                                              string                        Name,
                                              string                        Path,
                                              string[]                      RealmRoles,
                                              KeycloakGroupAccess           Access,
                                              Dictionary<string, string[]>  Attributes,
                                              KeycloakGroupDetailInternal[] Subgroups);

    #endregion
}