using System.Net;
using System.Text.Json;
using NUnit.Framework;

namespace KeycloakProvider.Tests;

public class UserGroupsProviderTests : BaseTests
{
    readonly KeycloakUsersProvider.KeycloakUserInternal user = new("1", Created, true, "firstName", "lastName", "a@found.com",
                                                                   new Dictionary<string, string[]>()
                                                                   {
                                                                       ["att"] = new[] {"value"}
                                                                   },
                                                                   Array.Empty<KeycloakUsersProvider.KeycloakUserFederatedIdentityInternal>());

    readonly KeycloakUserGroup group = new("1", "name", "/path");

    [Test]
    public async Task GetItems()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "users/" + user.id + "/groups",
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(new[] {group}))
                          });
        var provider = new KeycloakUserGroupsProvider(config, new HttpClient(mockHandler.Object));
        var groups   = await provider.GetItems(user.id);
        Assert.NotNull(groups);
        Assert.IsNotEmpty(groups);
    }

    [Test]
    public async Task AddTo()
    {
        AddMockedEndpoint(HttpMethod.Put,
                          "users/" + user.id + "/groups",
                          new HttpResponseMessage(HttpStatusCode.OK));
        var provider = new KeycloakUserGroupsProvider(config, new HttpClient(mockHandler.Object));
        var result   = await provider.AddTo(user.id, group.ID);
        Assert.True(result);
    }

    [Test]
    public async Task RemoveFrom()
    {
        AddMockedEndpoint(HttpMethod.Delete,
                          "users/" + user.id + "/groups",
                          new HttpResponseMessage(HttpStatusCode.OK));

        var provider = new KeycloakUserGroupsProvider(config, new HttpClient(mockHandler.Object));
        var result   = await provider.RemoveFrom(user.id, group.ID);
        Assert.True(result);
    }
}