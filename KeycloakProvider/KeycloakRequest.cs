namespace KeycloakProvider;

public abstract class KeycloakRequest
{
    internal readonly Dictionary<string, object> Values = new();
    
    internal object ToObject() => Values;
    
    public override string ToString() => string.Join("&", Values.Select(p => p.Key + "=" + p.Value));
}