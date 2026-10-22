public abstract class PlayerState : State<PlayerManager>
{
    protected PlayerState(
        PlayerManager player,
        StateMachine<PlayerManager> stateMachine)
        : base(player, stateMachine)
    {
    }
}
