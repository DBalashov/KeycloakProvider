namespace KeycloakProvider;

public sealed class KeycloakUpdateUser : KeycloakModifyUser
{
    public KeycloakUpdateUser Email(string email, bool emailVerified = true)
    {
        ArgumentNullException.ThrowIfNull(email);

        Values["email"]         = email;
        Values["emailVerified"] = emailVerified;

        return this;
    }
}