namespace KeycloakProvider;

public abstract class KeycloakModifyGroupRequest : KeycloakRequest, INamedEntity
{
    public string Name => (string) Values["name"];
}