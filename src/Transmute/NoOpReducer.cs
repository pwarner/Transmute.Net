namespace Transmute
{
    internal sealed class NoOpReducer<TState>: IReducer<TState>
    {
        public TState Reduce(TState state, object action) => state;
    }
}