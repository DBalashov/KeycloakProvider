namespace KeycloakProvider;

sealed class KeycloakUpdateRoleAttributes : KeycloakUpdateAttribute
{
    public KeycloakUpdateRoleAttributes(string roleName, Dictionary<string, object> attributes) : base(attributes) => 
        Values["name"] = roleName;
}