public abstract class EnemyState : State<EnemyManager>
{
    protected EnemyState(
        EnemyManager enemy,
        StateMachine<EnemyManager> stateMachine)
        : base(enemy, stateMachine)
    {
    }
}
