namespace KeycloakProvider;

/// <param name="Authority">url to server, including schema and ending by /realms/{realm} suffix, like "https://serverName.zone/realms/someRealm"</param>
/// <param name="UserName">user name in 'master' realm with 'admin' role</param>
/// <param name="AdminClientId">client id in 'master' realm</param>
/// <param name="AdminClientSecret">client secret in of AdminClientId (by default(?) == Psip5UvTO1EXUEwzb15nxLWnwdU1Nlcg)</param>
/// <param name="ServerUrl">if empty - take server name from Authority</param>
/// <param name="RequestTimeout">3 seconds if not specified</param>
public sealed record KeycloakProviderConfig(string   Realm,         string Authority,
                                            string   UserName,      string UserSecret,
                                            string   AdminClientId, string AdminClientSecret,
                                            string?  ServerUrl      = null,
                                            TimeSpan RequestTimeout = default) : KeycloakProviderAuthConfig(Realm,
                                                                                                            Authority,
                                                                                                            UserName,
                                                                                                            UserSecret,
                                                                                                            ServerUrl,
                                                                                                            RequestTimeout);