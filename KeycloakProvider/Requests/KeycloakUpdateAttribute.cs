namespace KeycloakProvider;

class KeycloakUpdateAttribute : KeycloakRequest
{
    public KeycloakUpdateAttribute(Dictionary<string, object> attributes) => Values["attributes"] = attributes;
}