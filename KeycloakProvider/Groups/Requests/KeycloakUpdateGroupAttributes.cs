namespace KeycloakProvider;

sealed class KeycloakUpdateGroupAttributes : KeycloakUpdateAttribute
{
    public KeycloakUpdateGroupAttributes(string groupName, Dictionary<string, object> attributes) : base(attributes) => 
        Values["name"] = groupName;
}