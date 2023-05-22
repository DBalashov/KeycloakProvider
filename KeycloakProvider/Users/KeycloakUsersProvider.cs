using System.Net;
using System.Net.Http.Json;

namespace KeycloakProvider;

sealed class KeycloakUsersProvider : BaseProvider, IKeycloakUsersProvider
{
    public KeycloakUsersProvider(KeycloakProviderConfig config) : base(config)
    {
    }

    public async Task<KeycloakUser?> GetById(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req  = await BuildMessage($"users/{userId}");
        var resp = await c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        var user = await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<KeycloakUserInternal>();
        return user == null ? null : convert(user);
    }

    public async Task<KeycloakUser[]> Find(bool exact, KeycloakFindUser parms)
    {
        ArgumentNullException.ThrowIfNull(parms);

        var fields = string.Join("&", parms.ToObject().Select(p => p.Key + "=" + p.Value));
        var req    = await BuildMessage($"users?exact={(exact ? "true" : "false")}&{fields}");
        var resp   = await c.SendAsync(req);
        var users  = await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<KeycloakUserInternal[]>();
        return (users ?? Array.Empty<KeycloakUserInternal>()).Select(convert).ToArray();
    }

    #region Create / Delete / Update

    public async Task Create(KeycloakCreateUser request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var req  = await BuildMessage("users", HttpMethod.Post, request.ToObject());
        var resp = await c.SendAsync(req);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<bool> Delete(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req  = await BuildMessage($"users/{userId}", HttpMethod.Delete);
        var resp = await c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return false;
        resp.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> Update(string userId, KeycloakUpdateUser request)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(request);

        if (!request.Values.Any()) throw new ArgumentException("Request empty");

        var req  = await BuildMessage($"users/{userId}", HttpMethod.Put, request.ToObject());
        var resp = await c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return false;
        resp.EnsureSuccessStatusCode();
        return true;
    }

    #endregion

    public async Task<KeycloakUserGroup[]> GetGroups(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req  = await BuildMessage($"users/{userId}/groups", HttpMethod.Get);
        var resp = await c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return Array.Empty<KeycloakUserGroup>();
        return await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<KeycloakUserGroup[]>() ?? Array.Empty<KeycloakUserGroup>();
    }

    #region ChangeState / UpdateAttributes

    public async Task<bool> ChangeState(string userId, bool newState)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req  = await BuildMessage($"users/{userId}", HttpMethod.Put, new {enabled = newState});
        var resp = await c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return false;

        resp.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> UpdateAttributes(string userId, Dictionary<string, string?> attributes)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(attributes);

        var user = await GetById(userId);
        if (user == null) return false;

        var existingAttributes = user.Attributes ?? new Dictionary<string, string>();
        foreach (var attr in attributes)
        {
            if (attr.Value == null) existingAttributes.Remove(attr.Key);
            else existingAttributes[attr.Key] = attr.Value;
        }

        var req  = await BuildMessage($"users/{userId}", HttpMethod.Put, new {attributes = existingAttributes});
        var resp = await c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return false;
        await resp.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
        return true;
    }

    #endregion

    public async Task<string[]> GetDisabled(params string[] userIds)
    {
        ArgumentNullException.ThrowIfNull(userIds);
        if (!userIds.Any()) return Array.Empty<string>();

        var req         = await BuildMessage("users?enabled=false");
        var resp        = await c.SendAsync(req);
        var respContent = await resp.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<KeycloakUserInternal[]>();
        return respContent!.Where(p => userIds.Contains(p.id)).Select(p => p.id).ToArray();
    }

    public async Task ResetPassword(string userId, string password)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(password);

        var req  = await BuildMessage($"users/{userId}/reset-password", HttpMethod.Put, new {type = "password", value = password, temporary = false});
        var resp = await c.SendAsync(req);
        resp.EnsureSuccessStatusCode();
    }

    public async Task Logout(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req  = await BuildMessage($"users/{userId}/logout", HttpMethod.Post);
        var resp = await c.SendAsync(req);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<KeycloakUserSession[]> GetSessions(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var req  = await BuildMessage($"users/{userId}/sessions", HttpMethod.Get);
        var resp = await c.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return Array.Empty<KeycloakUserSession>();

        resp.EnsureSuccessStatusCode();
        var items = await resp.Content.ReadFromJsonAsync<KeycloakUserSessionInternal[]>();

        return (items ?? Array.Empty<KeycloakUserSessionInternal>())
              .Select(p => new KeycloakUserSession(p.id, p.username, p.userid, p.ipaddress,
                                                   DateTime.UnixEpoch.AddMilliseconds(p.start),
                                                   DateTime.UnixEpoch.AddMilliseconds(p.lastAccess),
                                                   p.rememberMe,
                                                   p.clients))
              .ToArray();
    }

    #region internals

    KeycloakUser convert(KeycloakUserInternal from) =>
        new(from.id,
            DateTime.UnixEpoch.AddMilliseconds(from.createdTimestamp),
            from.enabled,
            from.firstName,
            from.lastName,
            from.email,
            from.attributes?.ToDictionary(p => p.Key, p => p.Value.First()),
            from.federatedIdentities?.Select(p => new KeycloakUserFederatedIdentity(p.identityProvider, p.userId, p.userName)).ToArray());

    sealed record KeycloakUserInternal(string                                  id,
                                       Int64                                   createdTimestamp,
                                       bool                                    enabled,
                                       string?                                 firstName,
                                       string?                                 lastName,
                                       string                                  email,
                                       Dictionary<string, string[]>?           attributes,
                                       KeycloakUserFederatedIdentityInternal[] federatedIdentities);

    sealed record KeycloakUserFederatedIdentityInternal(string identityProvider,
                                                        string userId,
                                                        string userName);

    sealed record KeycloakUserSessionInternal(string                     id,
                                              string                     username,
                                              string                     userid,
                                              string                     ipaddress,
                                              Int64                      start,
                                              Int64                      lastAccess,
                                              bool                       rememberMe,
                                              Dictionary<string, string> clients);

    #endregion
}