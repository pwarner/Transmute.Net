namespace Transmute.InvalidState
{
    public class ConstructorParameterMismatch
    {
        private readonly string _name;

        public ConstructorParameterMismatch(string name, int value)
        {
            _name = name;
            Value = value;
        }

        public int Value { get; }
    }
}