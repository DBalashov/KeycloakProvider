namespace KeycloakProvider;

/// <param name="Clients">ClientId : ClientName</param>
public sealed record KeycloakUserSession(string                     SessionId,
                                         string                     UserName,
                                         string                     UserId,
                                         string                     IPAddress,
                                         DateTime                   Started,
                                         DateTime                   LastAccess,
                                         bool                       RememberMe,
                                         Dictionary<string, string> Clients);