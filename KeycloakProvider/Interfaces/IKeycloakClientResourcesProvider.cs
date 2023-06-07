namespace KeycloakProvider;

public interface IKeycloakClientResourcesProvider
{
    Task<KeycloakClientResource[]> GetItems(string clientId);

    Task<KeycloakClientResource?> GetById(string clientId, string clientResourceId);
    
    Task<KeycloakClientResource>   Create(string   clientId, KeycloakCreateClientResource request);

    Task Delete(string clientId, string clientResourceId);
    
    Task<bool> Update(string clientId, KeycloakUpdateClientResource request);
}