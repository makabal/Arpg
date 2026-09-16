public sealed class PlayerNormalState : PlayerState
{
    public PlayerNormalState(
        PlayerManager player,
        StateMachine<PlayerManager> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Update()
    {
        Owner.ReadMovementInput();

        if (Owner.Input.AttackPressedThisFrame)
            StateMachine.ChangeState(Owner.AttackState);
    }

    public override void FixedUpdate()
    {
        Owner.ApplyMovement();
    }
}
