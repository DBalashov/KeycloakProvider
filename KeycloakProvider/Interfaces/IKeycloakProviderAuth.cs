namespace KeycloakProvider;

public interface IKeycloakProviderAuth
{
    Task<TokenContainer> GetToken(string userName, string userPassword);

    Task<TokenContainer> RefreshToken(string refreshToken);

    /// <summary>
    /// Find refresh token in TokensStore and refresh access token.
    /// Returned value is new access token and stored in TokensStore again.
    /// </summary>
    ValueTask<string?> GetToken(string accessToken);
}

public sealed record TokenContainer(string   AccessToken,
                                    DateTime AccessTokenExpired,
                                    string   RefreshToken,
                                    DateTime RefreshTokenExpired,
                                    string[] Scope,
                                    string   SessionState);