using System;
using Xunit;

namespace Transmute
{
    public class ReducerTests
    {
        [Theory]
        [ClassData(typeof(TestActions))]
        public void WithoutModificationReducersReturnOriginalState(object action)
        {
            Assert.Same(TestState.Before, Testable.Reduce(TestState.Before, action));
        }

        [Theory]
        [ClassData(typeof(TestActions))]
        public void DiscoveredReducerMethodsHandleActions(object action)
        {
            Assert.Same(TestState.After, Testable.Scan(typeof(ReducerMethods)).Reduce(TestState.Before, action));
        }

        [Fact]
        public void ThrowsArgumentNullExceptionIfActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Testable.Reduce(TestState.Before, null));
        }

        [Theory]
        [ClassData(typeof(TestActions))]
        public void OnMethodCanCreateReducerForActionWithDelegate(object action)
        {
            Reducer<TestState> reducer = Testable
                .On<TestAction1>((s, a) => TestState.After)
                .On<TestAction2>((s, a) => TestState.After);

            Assert.Same(TestState.After, reducer.Reduce(TestState.Before, action));
        }

        protected virtual Reducer<TestState> Testable =>
            new Reducer<TestState>();
    }
}