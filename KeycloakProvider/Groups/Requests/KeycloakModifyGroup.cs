using System.Diagnostics.CodeAnalysis;

namespace KeycloakProvider;

public abstract class KeycloakModifyGroupRequest : KeycloakRequest, INamedEntity
{
    [ExcludeFromCodeCoverage]
    public string Name => (string) Values["name"];
}