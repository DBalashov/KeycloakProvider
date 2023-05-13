namespace KeycloakProvider;

public sealed class KeycloakUpdateUser : KeycloakModifyUserRequest
{
    public KeycloakUpdateUser Email(string email, bool emailVerified = true)
    {
        ArgumentNullException.ThrowIfNull(email);

        Values["email"]         = email;
        Values["emailVerified"] = emailVerified;

        return this;
    }
}