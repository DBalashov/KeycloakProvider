namespace KeycloakProvider;

public sealed class KeycloakUpdateRole : KeycloakRequest
{
    public KeycloakUpdateRole ClientRole(bool value)
    {
        Values["clientRole"] = value;
        return this;
    }

    public KeycloakUpdateRole Description(string? description)
    {
        Values["description"] = description ?? "";
        return this;
    }
    
    public KeycloakUpdateRole Name(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Values["name"] = name;
        return this;
    }
}