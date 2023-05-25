namespace KeycloakProvider;

sealed class KeycloakUpdateRoleAttributes : KeycloakUpdateAttribute
{
    public KeycloakUpdateRoleAttributes(string groupName, Dictionary<string, object> attributes) : base(attributes) => 
        Values["name"] = groupName;
}