public sealed class PlayerDeadState : PlayerState
{
    public PlayerDeadState(
        PlayerManager player,
        StateMachine<PlayerManager> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        Owner.StopMovement();
    }
}
