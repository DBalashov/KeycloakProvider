using System.Diagnostics.CodeAnalysis;

namespace KeycloakProvider;

[ExcludeFromCodeCoverage]
public sealed record KeycloakUser(string                           ID,
                                  DateTime                         Created,
                                  bool                             Enabled,
                                  string?                          FirstName,
                                  string?                          LastName,
                                  string                           Email,
                                  Dictionary<string, string>?      Attributes,
                                  KeycloakUserFederatedIdentity[]? FederatedIdentities);

[ExcludeFromCodeCoverage]
public sealed record KeycloakUserFederatedIdentity(string IdentityProvider,
                                                   string UserId,
                                                   string UserName);

public enum KeycloakFilter
{
    Mail,
    MailVerified,
    FirstName,
    LastName
}