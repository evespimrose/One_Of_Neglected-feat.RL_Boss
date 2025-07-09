public abstract class BaseState<T> where T : class
{
    protected StateHandler<T> handler;

    protected BaseState(StateHandler<T> handler)
    {
        this.handler = handler;
    }

    public virtual void Enter(T entity) { }
    public virtual void Update(T entity) { }
    public virtual void Exit(T entity) { }
}