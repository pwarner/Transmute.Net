using System.Reflection;

namespace Transmute
{
    internal sealed class DelegatingReducer<TState, TAction> : IReducer<TState>
    {
        private readonly ReducerDelegate<TState, TAction> _delegate;

        public DelegatingReducer(MethodInfo reducerMethod) =>
            _delegate = (ReducerDelegate<TState, TAction>) reducerMethod.CreateDelegate(
                typeof(ReducerDelegate<TState,TAction>)
            );

        public DelegatingReducer(ReducerDelegate<TState, TAction> reducerDelegate) =>
            _delegate = reducerDelegate;

        public TState Reduce(TState state, object action) => 
            _delegate(state, (TAction)action);
    }
}