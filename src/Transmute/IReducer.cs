namespace Transmute
{
    public interface IReducer<TState>
    {
        TState Reduce(TState state, object action);
    }
}