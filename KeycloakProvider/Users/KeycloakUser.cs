namespace KeycloakProvider;

public sealed record KeycloakUser(string                           ID,
                                  DateTime                         Created,
                                  bool                             Enabled,
                                  string?                          FirstName,
                                  string?                          LastName,
                                  string                           Email,
                                  Dictionary<string, string>?      Attributes,
                                  KeycloakUserFederatedIdentity[]? FederatedIdentities);

public sealed record KeycloakUserFederatedIdentity(string IdentityProvider,
                                                   string UserId,
                                                   string UserName);

public sealed record KeycloakUserGroup(string ID, string Name, string Path);



public enum KeycloakFilter
{
    Mail,
    MailVerified,
    FirstName,
    LastName
}