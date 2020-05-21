using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Transmute.ReduxException;

namespace Transmute
{
    public sealed class ComplexTypeReducer<TState> : Reducer<TState>
    {
        private readonly IPropertyReducer<TState>[] _propertyReducers;
        private readonly ConstructorInfo _ctor;

        public ComplexTypeReducer()
        {
            Type stateType = typeof(TState);

            (ConstructorInfo ctor, ParameterInfo[] parameters) = GetConstructorAndParameters(stateType);
            _ctor = ctor;

            _propertyReducers = GetPropertyReducers(stateType, parameters).ToArray();
        }

        public ComplexTypeReducer<TState> SetPropertyReducer<TProp>(Expression<Func<TState, TProp>> selector,
            IReducer<TProp> reducer)
        {
            var property = (PropertyInfo) ((MemberExpression) selector.Body).Member;

            var propertyReducer =
                (IPropertyReducer<TState, TProp>) _propertyReducers.FirstOrDefault(p => p.Property == property);

            if (propertyReducer != null)
                propertyReducer.Reducer = reducer;

            return this;
        }

        protected override TState DefaultReducerBehaviour(TState state, object action)
        {
            (bool changed, object next)[] args = _propertyReducers
                .Select(p => p.ExecuteReducer(state, action))
                .ToArray();

            return args.Any(arg => arg.changed)
                ? (TState) _ctor.Invoke(args.Select(arg => arg.next).ToArray())
                : state;
        }

        private static (ConstructorInfo, ParameterInfo[]) GetConstructorAndParameters(Type stateType)
        {
            ConstructorInfo[] constructors = stateType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            ParameterInfo[] parameters = constructors.Length == 1
                ? constructors[0].GetParameters()
                : Array.Empty<ParameterInfo>();

            if (parameters.Length == 0)
                throw new ReduxException(InvalidCtor(stateType));

            return (constructors[0], parameters);
        }

        private static IEnumerable<IPropertyReducer<TState>> GetPropertyReducers(Type stateType, ParameterInfo[] parameters)
        {
            IReadOnlyDictionary<string, PropertyInfo> properties = stateType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            
            return parameters
                .Select(parameter => properties.TryGetValue(parameter.Name, out PropertyInfo property)
                    ? CreateReducerForProperty(stateType, property)
                    : throw new ReduxException(InvalidCtorArg(stateType, parameter.Name))
                );
        }

        private static IPropertyReducer<TState> CreateReducerForProperty(Type stateType, PropertyInfo property) =>
            (IPropertyReducer<TState>) Activator.CreateInstance(
                typeof(PropertyReducerAdapter<,>).MakeGenericType(stateType, property.PropertyType), 
                property
            );
    }
}