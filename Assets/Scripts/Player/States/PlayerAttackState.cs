public sealed class PlayerAttackState : PlayerState
{
    public PlayerAttackState(
        PlayerManager player,
        StateMachine<PlayerManager> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        Owner.Animation.PlayAttackAnimation();
    }

    public override void Update()
    {
        Owner.ReadMovementInput();
    }

    public override void FixedUpdate()
    {
        Owner.ApplyMovement();
    }

    public void AttackHit()
    {
        Owner.Combat.AttackHit();
    }

    public void FinishAttack()
    {
        StateMachine.ChangeState(Owner.NormalState);
    }
}
