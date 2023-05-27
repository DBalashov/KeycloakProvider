using System.Diagnostics.CodeAnalysis;

namespace KeycloakProvider;

/// <param name="Clients">ClientId : ClientName</param>
[ExcludeFromCodeCoverage]
public sealed record KeycloakUserSession(string                     SessionId,
                                         string                     UserName,
                                         string                     UserId,
                                         string                     IPAddress,
                                         DateTime                   Started,
                                         DateTime                   LastAccess,
                                         bool                       RememberMe,
                                         Dictionary<string, string> Clients);