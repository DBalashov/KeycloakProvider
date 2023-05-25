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
}