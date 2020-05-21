using Xunit;

namespace Transmute
{
    public class ComplexTypeReducerTests: ReducerTests
    {
        [Theory]
        [ClassData(typeof(TestActions))]
        public void CanComposeStateReducersAndDelegateToThem(object action)
        {
            ComplexTypeReducer<TestState> reducer = new ComplexTypeReducer<TestState>()
                .SetPropertyReducer(x => x.Name, new Reducer<string>().Scan(typeof(StateNameReducer)));

            TestState actual = reducer.Reduce(TestState.Before, action);

            string expected = action switch
            {
                TestAction1 a1 => StateNameReducer.OnTestAction1(TestState.Before.Name, a1),
                TestAction2 a2 => StateNameReducer.OnTestAction2(TestState.Before.Name, a2),
                _ => TestState.Before.Name
            };
                
            Assert.Equal(expected, actual.Name);
        }

        protected override Reducer<TestState> Testable => 
            new ComplexTypeReducer<TestState>();
    }
}