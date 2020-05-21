namespace Transmute.InvalidState
{
    public class TooManyPublicConstructors
    {
        public TooManyPublicConstructors(int value) => Value = value;

        public TooManyPublicConstructors(): this(13)
        {
        }

        public int Value { get; }
    }
}