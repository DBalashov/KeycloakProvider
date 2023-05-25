namespace KeycloakProvider;

public static class GroupsExtenders
{
    public static T Path<T>(this T o, string path) where T : KeycloakModifyGroupRequest
    {
        ArgumentNullException.ThrowIfNull(path);

        o.Values["path"] = path;
        return o;
    }

    public static T RealmRoles<T>(this T o, params string[] roles) where T : KeycloakModifyGroupRequest
    {
        ArgumentNullException.ThrowIfNull(roles);

        o.Values["realmRoles"] = roles;
        return o;
    }
}