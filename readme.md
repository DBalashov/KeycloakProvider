# Keycloak REST API Provider

## Roles management

```csharp
var config = new KeycloakProviderConfig("realmName", "https://some-keycloak-server/realms/realmName",
                                        "someAdminUser", "someAdminPassword",
                                        "admin-cli", "xxxxxxxxxxxxxxx");

var provider = new KeycloakProviderImp(config);

// create new role
await provider.Roles.Create(new KeycloakCreateRole("TestRole"));

// get role by name
var role = await provider.Roles.Get("TestRole");

// update role
await provider.Roles.Update(role.ID,
                            new KeycloakUpdateRole().Name("TestRole New Name")
                                                    .ClientRole(true)
                                                    .Description("Some description"));
// update attributes
await provider.Roles.Update(userId,
                            new KeycloakUpdateRole().Attributes(new Dictionary<string, string?>()
                                                                {
                                                                    ["a1"] = null, // null value means delete attribute
                                                                    ["attr1"] = "new value 1A"
                                                                }));
                                               
// delete role
await provider.Roles.Delete(role.ID);

// get realm roles
foreach (var roleItem in await provider.Roles.GetItems())
    Console.WriteLine($"{roleItem.ID}: {roleItem.Name}");

```

## Users management

```csharp
// create user
await provider.Users.Create(new KeycloakCreateUser("a@b.com").Password("123")
                                                             .Attributes(new Dictionary<string, string>()
                                                                         {
                                                                             ["attrib3"] = "value 3",
                                                                             ["attrib4"] = "value 4"
                                                                         }));
// find user by mail
var user = (await provider.Users.Find(true, new KeycloakFindUser(KeycloakFilter.Mail, "a@b.com"))).First();

// update user with state, password, name and attributes
await provider.Users.Update(user.ID, 
                            new KeycloakUpdateUser().Password("456")
                                                    .Name("First name", "Last name")
                                                    .Enabled(true)
                                                    .AddAttribute("attrib5", "value 5"));
// disable user
await provider.Users.Update(userId,
                            new KeycloakUpdateUser().Enabled(false));

// update attributes
await provider.Users.Update(userId,
                            new KeycloakUpdateUser().Attributes(new Dictionary<string, string?>()
                                                                {
                                                                    ["a1"] = null, // null value means delete attribute
                                                                    ["attr1"] = "new value 1A"
                                                                }));
// get user sessions
foreach(var session in await provider.Users.GetSessions(user.ID))
    Console.WriteLine($"{session.SessionId}: {session.Started}, {session.IPAddress}");

// disable user
await provider.Users.ChangeState(user.ID, false);

// get user groups
foreach(var g in await provider.Users.GetGroups(user.ID))
    Console.WriteLine($"{g.ID}: {g.Name}, {g.Path}");

// delete user
await provider.Users.Delete(user.ID);
```

## Groups management

```csharp
// create group with some attributes
await provider.Groups.Create(new KeycloakCreateGroup("Test group").AddAttribute("attr1", "value1"));

// get group list
var groups = await provider.Groups.GetItems();
foreach (var group in groups)
    Console.WriteLine($"{group.ID}: {group.Name}, {group.Path}");

// update group
var g = groups.First(p => p.Name == "Test group");
await provider.Groups.Update(g.ID,
                             new KeycloakUpdateGroup().AddAttribute("attr2", "value2"));

// update attributes
await provider.Groups.Update(g.ID,
                             new KeycloakUpdateGroup().Attributes(new Dictionary<string, string?>()
                                                                  {
                                                                      ["a1"] = null, // null value means delete attribute
                                                                      ["attr1"] = "new value 1A"
                                                                  }));
// delete group
await provider.Groups.Delete(g.ID);
```

## User-Groups management

```csharp
var user = (await provider.Users.Find(true, new KeycloakFindUser(KeycloakFilter.Mail, "a@b.com"))).First();
var groups = await provider.Groups.GetItems();
var isUserAddedToGroup = await provider.Users.AddToGroup(userId, groups.First().ID);
var isUserRemovedFromGroup = await provider.Users.RemoveFromGroup(userId, groups.First().ID);

```

## User-Roles management

```csharp
var user = (await provider.Users.Find(true, new KeycloakFindUser(KeycloakFilter.Mail, "a@b.com"))).First();
var roles = await provider.Roles.GetItems();
var role = roles.First(p => p.Name == "TestRole");
var isUserAssigned = await provider.UserRoles.Assign(userId, role.ID);
var isUserUnassigned = await provider.UserRoles.Unassign(userId, role.ID);
```

## Authenticate via API with Direct Access grants
    
```csharp
var config = new KeycloakProviderAuthConfig("realmName", "https://some-keycloak-server/realms/realmName",
                                            "web-api", "xxxxxxxxxxxxxx"); // client id and secret in realm
var kp = new KeycloakProviderAuthImp(config);
var accessToken = (await kp.GetToken("a@b.com", "12345678")).AccessToken;
```

for authenticate via DAG: client must be configured with Direct Access grants

![pic1.png](images/pic1.png)

Also, take client id and client secret from configuration
![pic2.png](images/pic2.png)