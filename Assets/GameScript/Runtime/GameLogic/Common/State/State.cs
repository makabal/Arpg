public abstract class State<T>
{
    protected T Owner { get; }
    protected StateMachine<T> StateMachine { get; }

    protected State(T owner, StateMachine<T> stateMachine)
    {
        Owner = owner;
        StateMachine = stateMachine;
    }

    public virtual void Enter()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
    }
}
