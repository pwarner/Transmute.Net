namespace Transmute
{
    internal static class ReducerMethods
    {
        private static TestState OnTestAction1(TestState state, TestAction1 action) => 
            TestState.After;

        private static TestState OnTestAction2(TestState state, TestAction2 action) =>
            TestState.After;
    }
}