using System.Diagnostics.CodeAnalysis;

namespace KeycloakProvider;

[ExcludeFromCodeCoverage]
public sealed record KeycloakUserGroup(string ID, string Name, string Path);