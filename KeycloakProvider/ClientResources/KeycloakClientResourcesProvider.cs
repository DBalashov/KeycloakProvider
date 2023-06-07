namespace KeycloakProvider;

sealed class KeycloakClientResourcesProvider : BaseProviderAdmin, IKeycloakClientResourcesProvider
{
    public KeycloakClientResourcesProvider(KeycloakProviderConfig config, HttpClient c) : base(config, c)
    {
    }

    public async Task<KeycloakClientResource[]> GetItems(string clientId)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        var req   = await BuildMessage($"clients/{clientId}/authz/resource-server/resource?deep=true", HttpMethod.Get);
        var items = await SendAndGetResponse<KeycloakClientResourceInternal[]>(req);
        return items!.Select(convert).ToArray();
    }

    public async Task<KeycloakClientResource?> GetById(string clientId, string clientResourceId)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(clientResourceId);

        var req = await BuildMessage($"clients/{clientId}/authz/resource-server/resource/{clientResourceId}", HttpMethod.Get);
        var r   = await SendAndGetResponse<KeycloakClientResourceInternal>(req);
        return r == null ? null : convert(r);
    }

    public async Task<KeycloakClientResource> Create(string clientId, KeycloakCreateClientResource request)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(request);

        var req = await BuildMessage($"clients/{clientId}/authz/resource-server/resource", HttpMethod.Post, request);
        var r   = await SendAndGetResponse<KeycloakClientResourceInternal>(req);
        return convert(r!);
    }

    public async Task Delete(string clientId, string clientResourceId)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(clientResourceId);

        var req = await BuildMessage($"clients/{clientId}/authz/resource-server/resource/{clientResourceId}", HttpMethod.Delete);
        await SendWithoutResponse(req);
    }

    public async Task<bool> Update(string clientId, KeycloakUpdateClientResource request)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(request);
        if (!request.Values.Any()) throw new ArgumentException(Errors.RequestEmpty);

        if (request.Values.TryGetValue("attributes", out var a) && a is Dictionary<string, string[]?> attrs)
        {
            var user = await GetById((string) request.Values["_id"], clientId);
            if (user == null) return false;

            attrs.MergeExistingAttributes(user.Attributes);
        }

        var req = await BuildMessage($"clients/{clientId}/authz/resource-server/resource", HttpMethod.Put, request);
        await SendWithoutResponse(req);
        return true;
    }

    #region internals

    KeycloakClientResource convert(KeycloakClientResourceInternal p) =>
        new(p._id, p.name, p.type, p.uris, p.ownerManagedAccess, p.displayName, p.scopes, p.owner,
            p.attributes?.ToDictionary(a => a.Key, a => a.Value.First()) ?? new Dictionary<string, string>());

    sealed record KeycloakClientResourceInternal(string                        _id,
                                                 string                        name,
                                                 string?                       type,
                                                 string[]                      uris,
                                                 bool                          ownerManagedAccess,
                                                 Dictionary<string, string[]>? attributes,
                                                 string?                       displayName,
                                                 KeycloakClientResourceScope[] scopes,
                                                 KeycloakClientResourceOwner   owner);

    #endregion
}