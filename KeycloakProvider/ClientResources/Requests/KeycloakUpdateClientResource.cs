namespace KeycloakProvider;

public class KeycloakUpdateClientResource : KeycloakRequest
{
    internal KeycloakUpdateClientResource()
    {
        
    } 

    public KeycloakUpdateClientResource(string clientResourceId)
    {
        ArgumentNullException.ThrowIfNull(clientResourceId);
        Values["_id"] = clientResourceId;
    }
}