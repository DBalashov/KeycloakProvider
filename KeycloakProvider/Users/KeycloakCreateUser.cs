namespace KeycloakProvider;

public sealed class KeycloakCreateUser : KeycloakModifyUserRequest
{
    public KeycloakCreateUser(string email, bool emailVerified = true)
    {
        ArgumentNullException.ThrowIfNull(email);

        Values["email"]         = email;
        Values["emailVerified"] = emailVerified;
    }
}