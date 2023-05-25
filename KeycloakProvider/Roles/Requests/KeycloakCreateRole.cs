namespace KeycloakProvider;

public sealed class KeycloakCreateRole : KeycloakModifyRole
{
    public KeycloakCreateRole(string name) => this.Name(name);
}