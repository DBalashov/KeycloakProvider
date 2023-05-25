namespace KeycloakProvider;

public static class Extenders
{
    public static T Attributes<T>(this T o, Dictionary<string, string> attributes) where T : IAttributableEntity
    {
        ArgumentNullException.ThrowIfNull(attributes);
        o.Values["attributes"] = attributes;
        return o;
    }

    public static T AddAttribute<T>(this T o, string name, string value) where T : IAttributableEntity
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);

        if (!o.Values.TryGetValue("attributes", out var attr))
            o.Values.Add("attributes", attr = new Dictionary<string, string>());

        ((Dictionary<string, string>) attr)[name] = value;
        return o;
    }

    public static T Name<T>(this T item, string name) where T : INamedEntity
    {
        ArgumentNullException.ThrowIfNull(name);
        item.Values["name"] = name;
        return item;
    }
}