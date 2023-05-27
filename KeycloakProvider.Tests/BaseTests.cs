using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace KeycloakProvider.Tests;

public abstract class BaseTests
{
    protected static Int64 Created = new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Subtract(DateTime.UnixEpoch).Ticks/1000;

    protected const string mockToken     = "access";
    protected const string mockAuthority = "https://auth";
    protected const string mockRealm     = "realm";
    protected const string mockUrl       = mockAuthority + "/admin/realms/" + mockRealm;
        
    protected readonly KeycloakProviderConfig config = new(mockRealm, mockAuthority, "user", "pass", "client", "secret");

    protected Mock<HttpMessageHandler> mockHandler = null!;

    [SetUp]
    public virtual void BeforeEachTest()
    {
        mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync",
                                             ItExpr.Is<HttpRequestMessage>(c => c.Method == HttpMethod.Post && c.RequestUri!.ToString().EndsWith("/realms/master/protocol/openid-connect/token")),
                                             ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                         {
                             Content = new StringContent(JsonSerializer.Serialize(new TokenStore.InternalTokenContainer(mockToken, "refresh", 3600, DateTime.MinValue)))
                         })
           .Verifiable();
    }

    protected bool Authorized(HttpRequestMessage p, HttpMethod method, string suffix) =>
        p.Method == method                                          &&
        p.RequestUri!.ToString().StartsWith(mockUrl + "/" + suffix) &&
        p.Headers.Authorization!.Scheme == "Bearer"                 && p.Headers.Authorization.Parameter == mockToken;
    
    protected void AddMockedEndpoint(HttpMethod method, string suffix, HttpResponseMessage? message = null)
    {
        mockHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync",
                                             ItExpr.Is<HttpRequestMessage>(p => Authorized(p, method, suffix)),
                                             ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(message ?? new HttpResponseMessage(HttpStatusCode.OK))
           .Verifiable();
    }
}