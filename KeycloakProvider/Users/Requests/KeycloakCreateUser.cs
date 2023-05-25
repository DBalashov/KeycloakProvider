namespace KeycloakProvider;

public sealed class KeycloakCreateUser : KeycloakModifyUser
{
    public KeycloakCreateUser(string email, bool emailVerified = true) => this.Email(email, emailVerified);
}