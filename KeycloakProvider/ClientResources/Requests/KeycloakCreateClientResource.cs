namespace KeycloakProvider;

public sealed class KeycloakCreateClientResource : KeycloakUpdateClientResource
{
    public KeycloakCreateClientResource(string name) => this.Name(name);
}