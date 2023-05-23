namespace KeycloakProvider;

sealed class KeycloakResetPassword : KeycloakRequest 
{
    public KeycloakResetPassword(string newPassword)
    {
        Values["type"]      = "password";
        Values["value"]     = newPassword;
        Values["temporary"] = false;
    }
}