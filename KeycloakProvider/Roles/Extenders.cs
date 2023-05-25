namespace KeycloakProvider;

public static class RolesExtenders
{
    public static T ClientRole<T>(this T o, bool value) where T : KeycloakModifyRole
    {
        o.Values["clientRole"] = value;
        return o;
    }

    public static T Description<T>(this T o, string? description) where T : KeycloakModifyRole
    {
        o.Values["description"] = description ?? "";
        return o;
    }
}