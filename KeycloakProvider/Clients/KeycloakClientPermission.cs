using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace KeycloakProvider;

[ExcludeFromCodeCoverage]
public sealed record KeycloakClientPermission(string  ID,
                                              string  Name,
                                              string? Description,
                                              string  Type,
                                              [property: JsonConverter(typeof(JsonStringEnumConverter))]
                                              ClientPermissionLogic Logic,
                                              [property: JsonConverter(typeof(JsonStringEnumConverter))]
                                              ClientPermissionDesisionStrategy DecisionStrategy);

public enum ClientPermissionDesisionStrategy
{
    AFFIRMATIVE,
    UNANIMOUS,
    CONSENSUS
}

public enum ClientPermissionLogic
{
    POSITIVE,
    NEGATIVE
}