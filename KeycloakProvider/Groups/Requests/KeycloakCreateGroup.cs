namespace KeycloakProvider;

public sealed class KeycloakCreateGroup : KeycloakModifyGroupRequest
{
    public string Name => (string) Values["name"];

    public KeycloakCreateGroup(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Values["name"] = name;
    }
}