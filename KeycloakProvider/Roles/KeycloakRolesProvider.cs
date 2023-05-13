using System.Net;
using System.Net.Http.Json;

namespace KeycloakProvider;

class KeycloakRolesProvider : BaseProvider, IKeycloakRolesProvider
{
    public KeycloakRolesProvider(ProviderSettings settings) : base(settings)
    {
    }

    public async Task<KeycloakRole[]> GetItems()
    {
        var req   = await BuildMessage("roles?briefRepresentation=false");
        var resp  = await settings.c.SendAsync(req);
        var roles = await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<KeycloakRoleInternal[]>();
        return (roles ?? Array.Empty<KeycloakRoleInternal>())
              .Select(p => new KeycloakRole(p.Id, p.Name, p.Description, p.Composite, p.ClientRole, p.ContainerId,
                                            p.Attributes.ToDictionary(a => a.Key, a => a.Value.First())))
              .ToArray();
    }

    public async Task<KeycloakRole?> Get(string roleName)
    {
        ArgumentNullException.ThrowIfNull(roleName);

        var req  = await BuildMessage("roles/" + roleName);
        var resp = await settings.c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;

        var p = await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<KeycloakRoleInternal>();
        return p == null
                   ? null
                   : new KeycloakRole(p.Id, p.Name, p.Description, p.Composite, p.ClientRole, p.ContainerId,
                                      p.Attributes.ToDictionary(a => a.Key, a => a.Value.First()));
    }

    public async Task Delete(string roleName)
    {
        ArgumentNullException.ThrowIfNull(roleName);

        var req  = await BuildMessage("roles/" + roleName, HttpMethod.Delete);
        var resp = await settings.c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return;
        resp.EnsureSuccessStatusCode();
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