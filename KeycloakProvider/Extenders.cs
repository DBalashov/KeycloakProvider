namespace KeycloakProvider;

public static class Extenders
{
    public static T Attributes<T>(this T o, Dictionary<string, string?> attributes) where T : KeycloakRequest
    {
        ArgumentNullException.ThrowIfNull(attributes);
        o.Values["attributes"] = attributes.ToDictionary(p => p.Key, p => p.Value == null ? null : new[] {p.Value});
        return o;
    }

    public static T Name<T>(this T item, string name) where T : KeycloakRequest
    {
        ArgumentNullException.ThrowIfNull(name);
        item.Values["name"] = name;
        return item;
    }

    public static T AddAttribute<T>(this T o, string name, string value) where T : KeycloakRequest
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);

        if (!o.Values.TryGetValue("attributes", out var attr))
            o.Values.Add("attributes", attr = new Dictionary<string, string[]>());

        ((Dictionary<string, string[]>) attr)[name] = new[] {value};
        return o;
    }

    public static void MergeExistingAttributes(this Dictionary<string, string[]?> newAttributes, Dictionary<string, string>? existingAttributes)
    {
        ArgumentNullException.ThrowIfNull(newAttributes);

        if (existingAttributes == null) return;
        foreach (var attr in existingAttributes)
            newAttributes.TryAdd(attr.Key, new[] {attr.Value});
    }
}