namespace KeycloakProvider;

sealed class KeycloakChangeState : KeycloakModifyUser
{
    public KeycloakChangeState(bool newState) => this.Enabled(newState);
}