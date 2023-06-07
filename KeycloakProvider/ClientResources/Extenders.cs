namespace KeycloakProvider;

public static class ClientResourceExtenders
{
    public static T DisplayName<T>(this T o, string? displayName) where T : KeycloakUpdateClientResource
    {
        o.Values["displayName"] = displayName!;
        return o;
    }

    public static T OwnerManagedAccess<T>(this T o, bool ownerManagedAccess) where T : KeycloakUpdateClientResource
    {
        o.Values["ownerManagedAccess"] = ownerManagedAccess;
        return o;
    }

    public static T Scopes<T>(this T o, KeycloakClientResourceScope[]? scopes) where T : KeycloakUpdateClientResource
    {
        o.Values["scopes"] = scopes?.Select(p => new {id = p.ID, name = p.Name}).ToArray()!;
        return o;
    }

    public static T URIs<T>(this T o, string[]? URIs) where T : KeycloakUpdateClientResource
    {
        o.Values["uris"] = URIs!;
        return o;
    }
    
    public static T Type<T>(this T o, string? type) where T : KeycloakUpdateClientResource
    {
        o.Values["type"] = type!;
        return o;
    }
}