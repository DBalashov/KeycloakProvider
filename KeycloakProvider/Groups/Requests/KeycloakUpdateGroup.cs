namespace KeycloakProvider;

public sealed class KeycloakUpdateGroup : KeycloakRequest
{
    public KeycloakUpdateGroup Id(string groupId)
    {
        ArgumentNullException.ThrowIfNull(groupId);

        Values["id"] = groupId;
        return this;
    }
}