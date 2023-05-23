namespace KeycloakProvider;

sealed class KeycloakChangeState : KeycloakRequest 
{
    public KeycloakChangeState(bool newState) => 
        Values["enabled"] = newState;
}
