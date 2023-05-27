using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace KeycloakProvider.Tests;

public class ProviderAuthTests : BaseTests
{
    [Test]
    public async Task GetToken()
    {
        var token = new KeycloakProviderAuthImp.InternalTokenContainer("access_token", "refresh_token", 3600, 7200, "state", "scope");

        var url = $"{mockAuthority}/realms/{config.Realm}/protocol/openid-connect/token";
        mockHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync",
                                             ItExpr.Is<HttpRequestMessage>(p => p.Method == HttpMethod.Post && p.RequestUri!.ToString() == url),
                                             ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                         {
                             Content = new StringContent(JsonSerializer.Serialize(token))
                         })
           .Verifiable();

        var p = new KeycloakProviderAuthImp(config, new HttpClient(mockHandler.Object));
        p.AttachTokenStore(new TokensStore(p));
        
        var r = await p.GetToken("user", "password");
        Assert.IsNotNull(r);
        Assert.IsNotNull(r.AccessToken);
        Assert.IsNotNull(r.RefreshToken);
        Assert.IsNotNull(r.Scope);
        Assert.IsTrue(r.Scope.Any());
        Assert.IsTrue(r.AccessTokenExpired  > DateTime.UtcNow);
        Assert.IsTrue(r.RefreshTokenExpired > DateTime.UtcNow);
    }
}