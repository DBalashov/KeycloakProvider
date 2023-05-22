namespace KeycloakProvider;

public interface IKeycloakProviderAuth
{
    Task<TokenContainer> Authenticate(string userName, string userPassword);

    Task<TokenContainer> Refresh(string refreshToken);
}

public sealed record TokenContainer(string   AccessToken,
                                    DateTime AccessTokenExpired,
                                    string   RefreshToken,
                                    DateTime RefreshTokenExpired,
                                    string[] Scope,
                                    string   SessionState);