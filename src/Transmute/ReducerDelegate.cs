namespace Transmute
{
    public delegate TState ReducerDelegate<TState, in TAction>(TState state, TAction action);
}