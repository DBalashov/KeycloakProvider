namespace KeycloakProvider;

public interface IKeycloakProviderAuth
{
    Task<TokenContainer> GetToken(string userName, string userPassword);

    Task<TokenContainer> RefreshToken(string refreshToken);

    ValueTask<string?> GetToken(string accessToken);
}

public sealed record TokenContainer(string   AccessToken,
                                    DateTime AccessTokenExpired,
                                    string   RefreshToken,
                                    DateTime RefreshTokenExpired,
                                    string[] Scope,
                                    string   SessionState);