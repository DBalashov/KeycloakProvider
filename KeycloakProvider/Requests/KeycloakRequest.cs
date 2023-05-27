using System.Text;
using System.Text.Json;

namespace KeycloakProvider;

public abstract class KeycloakRequest
{
    internal Dictionary<string, object> Values { get; } = new();

    internal virtual HttpContent AsHttpContent() => new StringContent(JsonSerializer.Serialize(Values), Encoding.UTF8, "application/json");
    
    public override string ToString() => string.Join("&", Values.Select(p => p.Key + "=" + p.Value));
}