using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Transmute
{
    public class Reducer<TState> : IReducer<TState>
    {
        private readonly Dictionary<Type, IReducer<TState>> _specificReducers;

        public Reducer()
        {
            _specificReducers = new Dictionary<Type, IReducer<TState>>();
        }

        public Reducer<TState> Scan(Type targetType)
        {
            foreach ((Type actionType, IReducer<TState> reducer) in ScanReducerForActionHandlers(targetType))
                Add(actionType, reducer);

            return this;
        }

        public TState Reduce(TState state, object action) =>
            TryGetReducerForActionType((action ?? throw new ArgumentNullException(nameof(action))).GetType(),
                out IReducer<TState> reducer)
                ? reducer.Reduce(state, action)
                : DefaultReducerBehaviour(state, action);

        public Reducer<TState> On<TAction>(ReducerDelegate<TState, TAction> reducer)
        {
            Add(typeof(TAction), new DelegatingReducer<TState, TAction>(reducer));
            return this; 
        }

        protected virtual TState DefaultReducerBehaviour(TState state, object action) => state;
        private void Add(Type actionType, IReducer<TState> reducer) =>
            _specificReducers[actionType] = reducer;

        private bool TryGetReducerForActionType(Type actionType, out IReducer<TState> reducer) => 
            _specificReducers.TryGetValue(actionType, out reducer);

        private static IEnumerable<(Type actionType, IReducer<TState> reducer)> ScanReducerForActionHandlers(IReflect thisType) =>
            from method in thisType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            let parameters = method.GetParameters()
            where HasReducerSignature(parameters, method.ReturnType)
            let actionType = parameters[1].ParameterType
            select (actionType, CreateReducer(actionType, method));

        private static bool HasReducerSignature(IReadOnlyList<ParameterInfo> parameters, Type returnType) =>
            parameters.Count == 2
            && parameters[0].ParameterType == typeof(TState)
            && returnType == typeof(TState);

        private static IReducer<TState> CreateReducer(Type actionType, MethodInfo method) =>
            (IReducer<TState>) Activator.CreateInstance(
                typeof(DelegatingReducer<,>).MakeGenericType(typeof(TState), actionType),
                method
            );
    }
}
