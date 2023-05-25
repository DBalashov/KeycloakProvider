namespace KeycloakProvider;

public static class Extenders
{
    public static T Attributes<T>(this T o, Dictionary<string, string> attributes) where T : IAttributableEntity
    {
        ArgumentNullException.ThrowIfNull(attributes);
        o.Values["attributes"] = attributes.ToDictionary(p => p.Key, p => (object) new[] {p.Value});
        return o;
    }

    public static T Name<T>(this T item, string name) where T : INamedEntity
    {
        ArgumentNullException.ThrowIfNull(name);
        item.Values["name"] = name;
        return item;
    }
}