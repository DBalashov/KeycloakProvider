using System.Net;
using System.Text.Json;
using NUnit.Framework;

namespace KeycloakProvider.Tests;

public class UsersProviderTests : BaseTests
{
    readonly KeycloakUsersProvider.KeycloakUserInternal user = new("1", Created, true, "firstName", "lastName", "a@found.com",
                                                                   new Dictionary<string, string[]>()
                                                                   {
                                                                       ["att"] = new[] {"value"}
                                                                   },
                                                                   Array.Empty<KeycloakUsersProvider.KeycloakUserFederatedIdentityInternal>());

    [Test]
    public async Task Find()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "users?exact=true&email=" + user.email + "&firstName=firstName&lastName=lastName&emailVerified=true",
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(new[] {user}))
                          });
        var p = new KeycloakUsersProvider(config, new HttpClient(mockHandler.Object));
        var r = await p.Find(true, new KeycloakFindUser(KeycloakFilter.Mail, user.email)
                                  .AddFilter(KeycloakFilter.FirstName,    "firstName")
                                  .AddFilter(KeycloakFilter.LastName,     "lastName")
                                  .AddFilter(KeycloakFilter.MailVerified, "true"));

        Assert.NotNull(r);
        Assert.IsTrue(r.Length == 1);
        Assert.IsNotNull(r[0]);
        Assert.IsTrue(r[0].ID        == user.id        &&
                      r[0].Enabled   == user.enabled   &&
                      r[0].FirstName == user.firstName && r[0].LastName == user.lastName &&
                      r[0].Email     == user.email);
    }

    [Test]
    public async Task GetById()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "users/" + user.id,
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(user))
                          });
        var p = new KeycloakUsersProvider(config, new HttpClient(mockHandler.Object));

        var r = await p.GetById(user.id);
        Assert.NotNull(r);
        Assert.IsTrue(r.ID        == user.id        &&
                      r.Enabled   == user.enabled   &&
                      r.FirstName == user.firstName && r.LastName == user.lastName &&
                      r.Email     == user.email);

        Assert.CatchAsync<ArgumentNullException>(() => p.GetById(null!));
    }

    [Test]
    public async Task Create()
    {
        AddMockedEndpoint(HttpMethod.Post, "users");

        var p = new KeycloakUsersProvider(config, new HttpClient(mockHandler.Object));
        await p.Create(new KeycloakCreateUser(user.email)
                      .Password("123")
                      .AddAttribute("attr1", "value1")
                      .Name("firstName", "lastName")
                      .UserName("username")
                      .Enabled(true));
    }

    [Test]
    public async Task Delete()
    {
        AddMockedEndpoint(HttpMethod.Delete, "users/" + user.id);

        var p = new KeycloakUsersProvider(config, new HttpClient(mockHandler.Object));
        await p.Delete(user.id);

        Assert.CatchAsync<ArgumentNullException>(() => p.Delete(null!));
    }

    [Test]
    public async Task Update()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "users/" + user.id,
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(user))
                          });
        AddMockedEndpoint(HttpMethod.Put, "users/" + user.id);

        var p = new KeycloakUsersProvider(config, new HttpClient(mockHandler.Object));
        await p.Update(user.id, new KeycloakUpdateUser().Password("123")
                                                        .Attributes(user.attributes!.ToDictionary(c => c.Key, c => c.Value[0])!));

        Assert.CatchAsync<ArgumentException>(() => p.Update(user.id, new KeycloakUpdateUser()));
    }

    [Test]
    public async Task ResetPassword()
    {
        AddMockedEndpoint(HttpMethod.Put, "users/" + user.id + "/reset-password");
        var p = new KeycloakUsersProvider(config, new HttpClient(mockHandler.Object));

        await p.ResetPassword(user.id, "123");
        Assert.CatchAsync<ArgumentNullException>(() => p.ResetPassword(null!,   "123"));
        Assert.CatchAsync<ArgumentNullException>(() => p.ResetPassword(user.id, null!));
    }

    [Test]
    public async Task Logout()
    {
        AddMockedEndpoint(HttpMethod.Post, "users/" + user.id + "/logout");
        var p = new KeycloakUsersProvider(config, new HttpClient(mockHandler.Object));

        await p.Logout(user.id);
        Assert.CatchAsync<ArgumentNullException>(() => p.Logout(null!));
    }

    [Test]
    public async Task GetDisabled()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "users?enabled=false",
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(new[] {user}))
                          });
        var p = new KeycloakUsersProvider(config, new HttpClient(mockHandler.Object));

        var r = await p.GetDisabled(user.id);
        Assert.IsNotNull(r);
        Assert.IsFalse(r.Except(new[] {user.id}).Any());

        var r2 = await p.GetDisabled();
        Assert.IsNotNull(r2);
        Assert.IsTrue(r2.Length == 0);

        Assert.CatchAsync<ArgumentNullException>(() => p.GetDisabled(null!));
    }

    [Test]
    public void FailedConfig()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "users",
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(new[] {user}))
                          });

        Assert.Catch<ArgumentNullException>(() => new KeycloakUsersProvider(null!, new HttpClient(mockHandler.Object)));
    }

    [Test]
    public void Models()
    {
        Assert.Catch<ArgumentNullException>(() => new KeycloakFindUser(KeycloakFilter.Mail, null!));

        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateUser(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateUser(user.email).Name(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateUser(user.email).Attributes(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateUser(user.email).Password(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateUser(user.email).UserName(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateUser(user.email).AddAttribute(null!,  "value"));
        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateUser(user.email).AddAttribute("name", null!));
    }
}