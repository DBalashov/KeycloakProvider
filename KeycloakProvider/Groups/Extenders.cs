namespace KeycloakProvider;

public static class GroupsExtenders
{
    public static T Name<T>(this T o, string name) where T : KeycloakModifyGroupRequest
    {
        ArgumentNullException.ThrowIfNull(name);
    
        o.Values["name"] = name;
        return o;
    }
    
    public static T Path<T>(this T o, string path) where T : KeycloakModifyGroupRequest
    {
        ArgumentNullException.ThrowIfNull(path);
    
        o.Values["path"] = path;
        return o;
    }
}