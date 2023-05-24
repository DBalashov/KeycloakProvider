namespace KeycloakProvider;

static class Errors
{
    public const string RequestEmpty            = "Request is empty.";
    public const string TokenStoreNotConfigured = "Token store not configured (must be pass in constructor)";
    public const string NotSupported            = "{0} not supported";
}