public sealed class EnemyHitState : EnemyState
{
    public EnemyHitState(
        EnemyManager enemy,
        StateMachine<EnemyManager> stateMachine)
        : base(enemy, stateMachine)
    {
    }

    public override void Enter()
    {
        Owner.Animation.PlayHitAnimation();
    }

    public override void Exit()
    {
        Owner.Animation.EndHitAnimation();
    }

    public void FinishHit()
    {
        StateMachine.ChangeState(Owner.IdleState);
    }
}
