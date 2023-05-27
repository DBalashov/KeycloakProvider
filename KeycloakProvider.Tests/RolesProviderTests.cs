using System.Net;
using System.Text.Json;
using NUnit.Framework;

namespace KeycloakProvider.Tests;

public class RolesProviderTests : BaseTests
{
    readonly KeycloakRolesProvider.KeycloakRoleInternal role = new("role-id", "role-name", "", false, true, "container-id", new Dictionary<string, string[]>()
                                                                                                                            {
                                                                                                                                ["attr"] = new[] {"value"}
                                                                                                                            });

    [Test]
    public async Task GetItems()
    {
        AddMockedEndpoint(HttpMethod.Get, "roles?briefRepresentation=false",
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(new[] {role}))
                          });
        var p = new KeycloakRolesProvider(config, new HttpClient(mockHandler.Object));
        var r = await p.GetItems();

        Assert.NotNull(r);
        Assert.IsTrue(r.Length == 1);
        Assert.IsNotNull(r[0]);
        Assert.IsTrue(r[0].ID          == role.Id          &&
                      r[0].Name        == role.Name        &&
                      r[0].Description == role.Description &&
                      r[0].Attributes  != null             && r[0].Attributes.ContainsKey("attr") && r[0].Attributes["attr"] == "value");
    }

    [Test]
    public async Task Get()
    {
        AddMockedEndpoint(HttpMethod.Get, "roles/" + role.Name,
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(role))
                          });
        var p = new KeycloakRolesProvider(config, new HttpClient(mockHandler.Object));
        var r = await p.Get(role.Name);

        Assert.CatchAsync<ArgumentNullException>(() => p.GetById(null!));

        Assert.NotNull(r);
        Assert.IsTrue(r.ID          == role.Id          &&
                      r.Name        == role.Name        &&
                      r.Description == role.Description &&
                      r.Attributes  != null             && r.Attributes.ContainsKey("attr") && r.Attributes["attr"] == "value");
    }

    [Test]
    public async Task GetById()
    {
        AddMockedEndpoint(HttpMethod.Get, "roles-by-id/" + role.Id,
                          new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(JsonSerializer.Serialize(role))
                          });
        var p = new KeycloakRolesProvider(config, new HttpClient(mockHandler.Object));
        var r = await p.GetById(role.Id);

        Assert.CatchAsync<ArgumentNullException>(() => p.GetById(null!));

        Assert.NotNull(r);
        Assert.IsTrue(r.ID          == role.Id          &&
                      r.Name        == role.Name        &&
                      r.Description == role.Description &&
                      r.Attributes  != null             && r.Attributes.ContainsKey("attr") && r.Attributes["attr"] == "value");
    }

    [Test]
    public async Task Delete()
    {
        AddMockedEndpoint(HttpMethod.Delete, "roles-by-id/" + role.Id);

        var p = new KeycloakRolesProvider(config, new HttpClient(mockHandler.Object));
        Assert.CatchAsync<ArgumentNullException>(() => p.Delete(null!));
        await p.Delete(role.Id);
    }

    [Test]
    public async Task Update()
    {
        AddMockedEndpoint(HttpMethod.Put, "roles-by-id/" + role.Id);

        var p = new KeycloakRolesProvider(config, new HttpClient(mockHandler.Object));
        Assert.CatchAsync<ArgumentException>(() => p.Update(role.Id, new KeycloakUpdateRole()));

        await p.Update(role.Id, new KeycloakUpdateRole().Name("new-name"));
    }

    [Test]
    public async Task Create()
    {
        AddMockedEndpoint(HttpMethod.Post, "roles");

        var p = new KeycloakRolesProvider(config, new HttpClient(mockHandler.Object));
        await p.Create(new KeycloakCreateRole(role.Name)
                      .AddAttribute("attr1", "value1")
                      .Description("desc")
                      .ClientRole(true));
    }

    [Test]
    public void FailedConfig()
    {
        AddMockedEndpoint(HttpMethod.Get, "roles", new HttpResponseMessage(HttpStatusCode.OK)
                                                   {
                                                       Content = new StringContent(JsonSerializer.Serialize(new[] {role}))
                                                   });

        Assert.Catch<ArgumentNullException>(() => new KeycloakRolesProvider(null!, new HttpClient(mockHandler.Object)));
    }

    [Test]
    public void Models()
    {
        Assert.Catch<ArgumentNullException>(() => new KeycloakCreateRole(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakUpdateRole().Name(null!));
        Assert.Catch<ArgumentNullException>(() => new KeycloakUpdateRole().Attributes(null!));
    }
}