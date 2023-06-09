﻿using System.Diagnostics.CodeAnalysis;

namespace KeycloakProvider;

sealed class KeycloakGroupsProvider : BaseProviderAdmin, IKeycloakGroupsProvider
{
    internal KeycloakGroupsProvider(KeycloakProviderConfig config, HttpClient c) : base(config, c)
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

        if (request.Values.TryGetValue("attributes", out var a) && a is Dictionary<string, string[]?> attrs)
        {
            var group = await GetById(groupId);
            if (group == null) return false;

            attrs.MergeExistingAttributes(group.Attributes);
        }

        var req = await BuildMessage($"groups/{groupId}", HttpMethod.Put, request);
        return await SendWithoutResponse(req);
    }

    #region internals

    KeycloakGroupDetail convert(KeycloakGroupDetailInternal from) =>
        new(from.ID, from.Name, from.Path, from.RealmRoles, from.Access,
            from.Attributes.ToDictionary(p => p.Key, p => p.Value[0]),
            from.Subgroups.Select(convert).ToArray());

    [ExcludeFromCodeCoverage]
    internal sealed record KeycloakGroupDetailInternal(string                        ID,
                                                       string                        Name,
                                                       string                        Path,
                                                       string[]                      RealmRoles,
                                                       KeycloakGroupAccess           Access,
                                                       Dictionary<string, string[]>  Attributes,
                                                       KeycloakGroupDetailInternal[] Subgroups);

    #endregion
}