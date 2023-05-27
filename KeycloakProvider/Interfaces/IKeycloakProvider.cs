namespace KeycloakProvider;

public interface IKeycloakProvider
{
    /// <summary> Users related operations </summary>
    IKeycloakUsersProvider Users { get; }

    /// <summary> Groups related operations </summary>
    IKeycloakGroupsProvider Groups { get; }

    /// <summary> Roles related operations </summary>
    IKeycloakRolesProvider Roles { get; }
    
    /// <summary> User/groups related operations </summary>
    IKeycloakUserGroupsProvider UserGroups { get; }
}

public interface INamedEntity
{
    
}