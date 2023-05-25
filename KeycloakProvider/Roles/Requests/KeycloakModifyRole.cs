namespace KeycloakProvider;

public abstract class KeycloakModifyRole : KeycloakRequest, INamedEntity
{
    public string Name => (string) Values["name"];
}