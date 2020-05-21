using System.Reflection;

namespace Transmute
{
    internal interface IPropertyReducer<in TState>
    {
        PropertyInfo Property { get; }
        (bool changed, object next) ExecuteReducer(TState state, object action);
    }

    internal interface IPropertyReducer<in TState, TProp> : IPropertyReducer<TState>
    {
        IReducer<TProp> Reducer { get; set; }
        new(bool changed, TProp next) ExecuteReducer(TState state, object action);
    }
}