### Transmute.Net

Transmute.Net is a lightweight utility for reliable transformation of application state, inspired by [redux.js](https://redux.js.org/).

There's a wealth of great information about the core concepts on that site, so please do go have a read. But I'll outline the core concepts here.

##### The concept of Reducers
```csharp
public delegate TState ReducerDelegate<TState, in TAction>(TState state, TAction action);
```

A reducer is a pure function that takes an initial state value and an action value, and returns a new state value.
Reducers are:
- synchronous
- idempotent (no side effects)
- composable (to match your composable application state)

It is very important to understand here that both *state* and *action* are immutable. This is the very essence of what a reducer *is*: for calculating a new, independent state value from a previous value plus an action. Reducers never modify their two inputs.

If you are LINQ savvy, then you might recognise this sort of signature from LINQ aggregation.
```csharp
public static TAccumulate Aggregate<TSource,TAccumulate> (this System.Collections.Generic.IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate,TSource,TAccumulate> func);
```
Semantically, aggregation and reduction sound at odds, yet they refer to the same process. 

The term *Reduce* comes from functional programming, where most complex systems can be broken down to **map** & **reduce** functions.

*Aggregation* refers to the accumulator value 'aggregating' (i.e. collecting) multiple input values.

When you set out to roll a giant snowball, you are *aggregating* a snowball, while *reducing* the available snow.

##### Time Travel

You can view your application state at a given point in time as an aggregation of all state-changing actions at that point of time, with your application-state as the accumulator.

The application reducer is the aggregation function, called recursively, with arguments of each subsequently returned state, and the next action in the timeline.

Not only can you understand what your application state was at a set point in time, but you can support BI in asking questions such as "what would the state be if these events has not occured?"

#### Shut up and show me some code
The `IReducer<TState>` interface is defined as follows:

```csharp
public interface IReducer<TState>
{
    TState Reduce(TState state, object action);
}
```
There are two public, concrete implementations of this interface: 
```csahrp
public class Reducer<TState> : IReducer<TState>
{
    // used for simple types
}

public sealed class ComplexTypeReducer<TState> : Reducer<TState>
{
    /// builds on above type, used for complex types
}
```

Let's look at the capabilities of `Reducer<TState>` first.

```csharp
public sealed class IncrementAction
{
    public IncrementAction(uint value) => Value = value;

    public uint Value { get; }
}

public sealed class DecrementAction
{
    public DecrementAction(uint value) => Value = value;

    public uint Value { get; }
}

[Fact]
public void CanCreateReducerDelegatesForSpecificActionTypes()
{
    var reducer = new Reducer<int>()
        .On<IncrementAction>((state, increment) => state + increment.Value)
        .On<DecrementAction>((state, decrement) => state - decrement.Value);

    const int beginState = 13;

    Assert.Equal(beginState + 4, reducer.Reduce(beginState, new IncrementAction(4));
    Assert.Equal(beginState - 3, reducer.Reduce(beginState, new DecrementAction(3));
}
```

In this example, I use a simple, primitive type of System.INT32 because it's immutable. 
I'd be surprised if your application state was that simple, so we'll look at complex types shortly.

In this example we call the `On<TAction>` method to set a reducer function for an action of a specific type.

```csharp
public Reducer<TState> On<TAction>(ReducerDelegate<TState, TAction> reducer)
{ 
}
```

Use **`_`** discards for cases where your reducer output state doesn't require either input state or action to be calculated:

```csharp

public sealed class IncrementByOneAction
{
}

public sealed class SetNumericValueAction
{
    public SetNumericValueAction(int value) => Value = value;

    public int Value { get; }
}

[Fact]
public void CanCreateReducerDelegatesForSpecificActionTypes()
{
    var reducer = new Reducer<int>()
        .On<IncrementByOneAction>((state, _) => state + 1)
        .On<SetNumericAction>((_, action) => action.Value);

    const int beginState = 13;
    Assert.Equal(beginState + 1, reducer.Reduce(beginState, new IncrementByOneAction());

    const int expextedState = 25;
    Assert.Equal(expectedState, reducer.Reduce(beginState, new SetNumericValueAction(expectedState));
}
```

##### Scan for methods
Building up reducer behaviour doesn't require that you use lambda-style delegates - it can also be done by scanning a class for reducer methods.

```csharp
public static class SimpleIntReducers
{
    private static int Increment(int state, IncrementAction increment) =>
        state + increment.Value;

    private static int Decrement(int state, DecrementAction decrement) =>
        state - decrement.Value;
}

[Fact]
public void CanUseDerivedReducerMethods()
{
    var reducer = new Reducer<int>().Scan(typeof(SimpleIntReducers));

    const int beginState = 13;

    Assert.Equal(beginState + 4, reducer.Reduce(beginState, new IncrementAction(4));
    Assert.Equal(beginState - 3, reducer.Reduce(beginState, new DecrementAction(3));
}
```

For derived reducer methods to be discovered, they must be static and match the reducer signature for strongly typed actions. The methods can be declared private or public.

You can acquire reducer behaviours via a mix of the inline `On<TAction>` methods and type scanning. Reducers for each action type always overwrite any previous reducers for that action type.

So you could - for example - scan a class for reducer methods, then overwrite with the `On<TAction>` method for a specific action type.

It probably doesn't make a lot of sense to mix these approaches, but the flexibility exists nonetheless.

#### Composing reducers for complex types

Given a (admittedly not very) complex type ...
```csharp
public sealed class Person
{
    public Person(int age, string name)
    {
        Age = age;
        Name = name;
    }

    public int Age { get; }
    public string Name { get; }
}
```

... you can use a `ComplexTypeReducer<T>` to compose reducers for each of the properties in our type.

```csharp
public sealed class HadABirthday
{
}

public sealed class NameChanged
{
    public NameChanged(string newName) => NewName = newName;
    public string NewName { get; }
}

[Fact]
public void CanComposeReducersForComplexState()
{
    Reducer<string> nameReducer = new Reducer<string>()
        .On((NameChanged nameChanged) => nameChanged.NewName);

    Reducer<int> ageReducer = new Reducer<int>()
        .On<HadABirthday>((int age) => age + 1;

    Reducer<Person> personReducer = new Reducer<Person>
        .SetPropertyReducer(p=> p.Name, nameReducer)
        .SetPropertyReducer(p=> p.Age, ageReducer)

    var beginPerson = new Person(42, "Elmer Fudd");

    var nextPerson = personReducer.Reduce(beginPerson, new HadABirthday());
    Assert.NotSame(beginPerson, nextPerson);
    Assert.Equal(beginPerson.Age + 1, nextPerson.Age);
    Assert.Equal(beginPerson.Name, nextPerson.Name);

    const string newName = "Yosemite Sam";
    var anotherPerson = personReducer.Reduce(nextPerson, new NameChanged(newName));
    Assert.NotSame(nextPerson, anotherPerson);
    Assert.Equal(nextPerson.Age, anotherPerson.Age);
    Assert.Equal(newName, anotherPerson.Name);
}
```
In the example above, `SetPropertyReducer` is used to set an instance of `IReducer<TPropertyType>` to use to reduce that property's value.

You don't have to specify a reducer for each property - by default, a No-Op reducer is set for each property. That is, a reducer that just returns the state it was given unchanged.

In order to use a `ComplexTypeReducer<TState>` then the complex-type `TState` must obey the following rules:
- it must declare a single, public constructor.
- the constructor must declare at least one parameter.
- each parameter name must match a public, gettable property name (name comparison is case-insentive).

This ensures that your instances of state are **immutable** : State changes are only possible by constructing a new instance of state, never by modification of existing state.
If you try to use a `ComplexStateReducer<TState>` with an incompatible type, construction of this reducer type will throw a `ReduxException`.