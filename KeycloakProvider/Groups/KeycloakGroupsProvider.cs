using System.Net;
using System.Net.Http.Json;

namespace KeycloakProvider;

class KeycloakGroupsProvider : BaseProvider, IKeycloakGroupsProvider
{
    public KeycloakGroupsProvider(ProviderSettings settings) : base(settings)
    {
    }

    public async Task<KeycloakGroup[]> GetItems()
    {
        var req    = await BuildMessage("groups");
        var resp   = await settings.c.SendAsync(req);
        var groups = await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<KeycloakGroup[]>();
        return groups ?? Array.Empty<KeycloakGroup>();
    }

    public async Task<KeycloakGroupDetail?> GetById(string groupId)
    {
        ArgumentNullException.ThrowIfNull(groupId);
        
        var req  = await BuildMessage($"groups/{groupId}");
        var resp = await settings.c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;

        var group = await resp.Content.ReadFromJsonAsync<KeycloakGroupDetailInternal>();
        return group != null ? convert(group) : null;
    }
    
    public async Task<bool> Delete(string groupId)
    {
        ArgumentNullException.ThrowIfNull(groupId);
        
        var req  = await BuildMessage($"groups/{groupId}", HttpMethod.Delete);
        var resp = await settings.c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return false;
        resp.EnsureSuccessStatusCode();
        return true;
    }
    
    public async Task Create(KeycloakUpdateGroup request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!request.Values.Any()) throw new ArgumentException("Request empty");
        
        var req  = await BuildMessage("groups", HttpMethod.Post, request.ToObject());
        var resp = await settings.c.SendAsync(req);
        resp.EnsureSuccessStatusCode();
    }
    
    public async Task<bool> Update(string groupId, KeycloakUpdateGroup request)
    {
        ArgumentNullException.ThrowIfNull(groupId);
        ArgumentNullException.ThrowIfNull(request);
        if (!request.Values.Any()) throw new ArgumentException("Request empty");
        
        var req  = await BuildMessage($"groups/{groupId}", HttpMethod.Put, request.ToObject());
        var resp = await settings.c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return false;
        resp.EnsureSuccessStatusCode();
        return true;
    }

    KeycloakGroupDetail convert(KeycloakGroupDetailInternal from) =>
        new(from.ID, from.Name, from.Path, from.RealmRoles, from.Access,
            from.Attributes.ToDictionary(p => p.Key, p => p.Value[0]),
            from.Subgroups.Select(convert).ToArray());

    sealed record KeycloakGroupDetailInternal(string                        ID,
                                              string                        Name,
                                              string                        Path,
                                              string[]                      RealmRoles,
                                              KeycloakGroupAccess Access,
                                              Dictionary<string, string[]>  Attributes,
                                              KeycloakGroupDetailInternal[] Subgroups);
}