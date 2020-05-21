using System;
using System.Collections.Generic;
using System.Reflection;

namespace Transmute
{
    internal class PropertyReducerAdapter<TState, TProp>: IPropertyReducer<TState, TProp>
    {
        public PropertyReducerAdapter(PropertyInfo property)
        {
            Property = property;
            Reducer = (IReducer<TProp>) Activator.CreateInstance(typeof(NoOpReducer<>)
                .MakeGenericType(property.PropertyType));
        }

        public PropertyInfo Property { get; }
        public IReducer<TProp> Reducer { get; set; }

        public (bool changed, TProp next) ExecuteReducer(TState state, object action)
        {
            var current = (TProp) Property.GetValue(state, null);
            TProp next = Reducer.Reduce(current, action);
            return (!EqualityComparer<TProp>.Default.Equals(current, next), next);
        }

        (bool changed, object next) IPropertyReducer<TState>.ExecuteReducer(TState state, object action) => 
            ExecuteReducer(state, action);
    }
}