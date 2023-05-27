using System.Text;
using System.Text.Json;

namespace KeycloakProvider;

sealed class KeycloakUserRoleUnassign : KeycloakRequest
{
    readonly List<UserRoleItem> roles = new();

    internal override HttpContent AsHttpContent() => new StringContent(JsonSerializer.Serialize(roles), Encoding.UTF8, "application/json");

    public KeycloakUserRoleUnassign AddRoles(IEnumerable<KeycloakRole> roles)
    {
        foreach (var item in roles)
            this.roles.Add(new UserRoleItem(item.ID, item.Name));

        return this;
    }

    sealed record UserRoleItem(string id, string name);
}