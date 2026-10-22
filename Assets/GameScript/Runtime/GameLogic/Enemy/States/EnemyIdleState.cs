public sealed class EnemyIdleState : EnemyState
{
    public EnemyIdleState(
        EnemyManager enemy,
        StateMachine<EnemyManager> stateMachine)
        : base(enemy, stateMachine)
    {
    }
}
