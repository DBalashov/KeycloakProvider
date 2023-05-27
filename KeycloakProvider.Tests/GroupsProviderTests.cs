using System.Net;
using System.Text.Json;
using NUnit.Framework;

namespace KeycloakProvider.Tests;

public class GroupsProviderTests : BaseTests
{
    readonly KeycloakGroupsProvider.KeycloakGroupDetailInternal group = new("group-id", "group-name", "", Array.Empty<string>(),
                                                                            new KeycloakGroupAccess(true, true, true),
                                                                            new Dictionary<string, string[]>(),
                                                                            Array.Empty<KeycloakGroupsProvider.KeycloakGroupDetailInternal>());

    [Test]
    public async Task GetItems()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "groups",
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(new[] {group}))
                          });
        var p = new KeycloakGroupsProvider(config, new HttpClient(mockHandler.Object));
        var r = await p.GetItems();

        Assert.NotNull(r);
        Assert.IsTrue(r.Length == 1);
        Assert.IsNotNull(r[0]);
        Assert.IsTrue(r[0].ID   == group.ID   &&
                      r[0].Name == group.Name &&
                      r[0].Path == group.Path &&
                      r[0].Subgroups is {Length: 0});
    }

    [Test]
    public async Task GetById()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "groups/" + group.ID,
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(group))
                          });
        var p = new KeycloakGroupsProvider(config, new HttpClient(mockHandler.Object));
        var r = await p.GetById(group.ID);

        Assert.NotNull(r);
        Assert.IsTrue(r.ID   == group.ID   &&
                      r.Name == group.Name &&
                      r.Path == group.Path &&
                      r.Subgroups is {Length: 0});
    }

    [Test]
    public async Task Create()
    {
        AddMockedEndpoint(HttpMethod.Post, "groups");

        var p = new KeycloakGroupsProvider(config, new HttpClient(mockHandler.Object));
        await p.Create(new KeycloakCreateGroup(group.Name)
                          .AddAttribute("attr1", "value1"));
    }

    [Test]
    public async Task Delete()
    {
        AddMockedEndpoint(HttpMethod.Delete, "groups");

        var p = new KeycloakGroupsProvider(config, new HttpClient(mockHandler.Object));
        await p.Delete(group.ID);

        Assert.CatchAsync<ArgumentNullException>(() => p.Delete(null!));
    }

    [Test]
    public async Task Update()
    {
        AddMockedEndpoint(HttpMethod.Put, "groups/" + group.ID);
        AddMockedEndpoint(HttpMethod.Get,
                          "groups/" + group.ID,
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(group))
                          });

        var p = new KeycloakGroupsProvider(config, new HttpClient(mockHandler.Object));
        Assert.CatchAsync<ArgumentException>(() => p.Update(group.ID, new KeycloakUpdateGroup()));

        await p.Update(group.ID, new KeycloakUpdateGroup().Name("new-name")
                                                          .Path("/new-path")
                                                          .RealmRoles("role1", "role2")
                                                          .Attributes(new Dictionary<string, string?>()
                                                                      {
                                                                          ["attr1"] = "value"
                                                                      }));
    }

    [Test]
    public void FailedConfig()
    {
        AddMockedEndpoint(HttpMethod.Get,
                          "groups",
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(new[] {group}))
                          });

        Assert.Catch<ArgumentNullException>(() => new KeycloakGroupsProvider(null!, new HttpClient(mockHandler.Object)));
    }

    [Test]
    public void Models()
    {
        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateGroup(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakUpdateGroup().Name(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakUpdateGroup().AddAttribute(null!,  "value"));
        Assert.Catch<ArgumentNullException>(() => new KeycloakUpdateGroup().AddAttribute("attr", null!));
    }
}