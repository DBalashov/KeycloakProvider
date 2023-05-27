namespace KeycloakProvider;

public sealed class KeycloakCreateGroup : KeycloakModifyGroupRequest
{
    public KeycloakCreateGroup(string name) => this.Name(name);
}