namespace KeycloakProvider;

public sealed class KeycloakFindUser
{
    readonly Dictionary<string, string> filters = new();

    public KeycloakFindUser(KeycloakFilter filter, string value) =>
        AddFilter(filter, value);

    public KeycloakFindUser AddFilter(KeycloakFilter filter, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var field = filter switch
                    {
                        KeycloakFilter.Mail         => "email",
                        KeycloakFilter.MailVerified => "emailVerified",
                        KeycloakFilter.FirstName    => "firstName",
                        KeycloakFilter.LastName     => "lastName",
                        _                           => throw new NotSupportedException(string.Format(Errors.NotSupported, filter))
                    };

        filters[field] = value;
        return this;
    }

    internal string AsQueryString() => string.Join("&", filters.Select(p => p.Key + "=" + p.Value));

    public override string ToString() => string.Join("&", filters.Select(p => p.Key + "=" + p.Value));
}