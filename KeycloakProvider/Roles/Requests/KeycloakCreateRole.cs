namespace KeycloakProvider;

public sealed class KeycloakCreateRole : KeycloakRequest
{
    public string Name => (string) Values["name"];

    public KeycloakCreateRole(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Values["name"] = name;
    }
}