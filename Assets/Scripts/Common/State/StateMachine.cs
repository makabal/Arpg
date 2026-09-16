public sealed class StateMachine<T>
{
    public State<T> CurrentState { get; private set; }

    public void ChangeState(State<T> nextState)
    {
        if (nextState == null || ReferenceEquals(nextState, CurrentState))
            return;

        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}
