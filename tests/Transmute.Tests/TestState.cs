using System;

namespace Transmute
{
    public sealed class TestState: IEquatable<TestState>
    {
        private readonly int _hashCode;

        public TestState(string name, int age, bool active)
        {
            Name = name;
            Age = age;
            Active = active;
            _hashCode = HashCode.Combine(Name, Age, Active);
        }
        
        public string Name { get; }
        
        public int Age { get; }
        
        public bool Active { get; }

        public bool Equals(TestState other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            return Name == other.Name && Age == other.Age && Active == other.Active;
        }

        public override bool Equals(object obj) => obj is TestState other && Equals(other);

        public override int GetHashCode() => _hashCode;

        public static readonly TestState Before = new TestState("Test", 13, false);

        public static readonly TestState After = new TestState("Foo", 99, true);
    }
}