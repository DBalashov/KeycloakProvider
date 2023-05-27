using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace KeycloakProvider;

sealed class KeycloakUserRoleAssign : KeycloakRequest
{
    readonly List<UserRoleItem> roles = new();

    internal override HttpContent AsHttpContent() => new StringContent(JsonSerializer.Serialize(roles), Encoding.UTF8, "application/json");

    public KeycloakUserRoleAssign AddRoles(IEnumerable<KeycloakRole> roles)
    {
        foreach (var item in roles)
            this.roles.Add(new UserRoleItem(item.ID, item.Name, item.ClientRole, item.Composite, item.ContainerId));

        return this;
    }

    [ExcludeFromCodeCoverage]
    sealed record UserRoleItem(string id,
                               string name,
                               bool   clientRole,
                               bool   composite,
                               string containerId);
}