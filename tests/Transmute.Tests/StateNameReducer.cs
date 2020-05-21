namespace Transmute
{
    internal static class StateNameReducer
    {
        public static string OnTestAction1(string name, TestAction1 action) =>
            $"{name}+{nameof(TestAction1)}";

        public static string OnTestAction2(string name, TestAction2 action) =>
            $"{name}+{nameof(TestAction2)}";
    }
}