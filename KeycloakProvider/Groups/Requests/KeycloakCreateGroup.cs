namespace KeycloakProvider;

public sealed class KeycloakCreateGroup : KeycloakModifyGroupRequest
{
    public KeycloakCreateGroup(string name) => this.Name(name);

    public KeycloakCreateGroup AddAttribute(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);

        if (!Values.TryGetValue("attributes", out var attr))
            Values.Add("attributes", attr = new Dictionary<string, object>());

        ((Dictionary<string, object>) attr)[name] = new[] {value};
        return this;
    }
}