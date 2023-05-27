namespace KeycloakProvider;

public interface IKeycloakUserRolesProvider
{
    Task<UserRoleItem[]> GetItems(string userId);

    Task<bool> Assign(string userId, params string[] roleIds);
    
    Task<bool> Unassign(string userId, params string[] roleIds);
}