namespace KeycloakProvider;

sealed class KeycloakUpdateAttribute : KeycloakRequest 
{
    public KeycloakUpdateAttribute(Dictionary<string, string> attributes) => 
        Values["attributes"] = attributes;
}