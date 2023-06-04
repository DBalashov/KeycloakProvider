namespace KeycloakProvider;

sealed class KeycloakClientsProvider : BaseProviderAdmin, IKeycloakClientsProvider
{
    public KeycloakClientsProvider(KeycloakProviderConfig config, HttpClient c) : base(config, c)
    {
    }

    public async Task<KeycloakClientRole[]> GetRoles(string clientId)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        var req   = await BuildMessage($"clients/{clientId}/roles", HttpMethod.Get);
        var items = await SendAndGetResponse<KeycloakClientRole[]>(req);
        return items!;
    }

    public async Task<KeycloakClientScope[]> GetScopes()
    {
        var req   = await BuildMessage("client-scopes", HttpMethod.Get);
        var items = await SendAndGetResponse<KeycloakClientScope[]>(req);
        return items!;
    }

    public async Task<KeycloakClientPermission[]> GetPermissions(string clientId)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        var req   = await BuildMessage($"clients/{clientId}/authz/resource-server/permission", HttpMethod.Get);
        var items = await SendAndGetResponse<KeycloakClientPermission[]>(req);
        return items!;
    }
}