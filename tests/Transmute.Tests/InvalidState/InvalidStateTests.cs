using Xunit;
using static Transmute.ReduxException;

namespace Transmute.InvalidState
{
    public class InvalidStateTests
    {
        [Fact]
        public void ThrowsIfNoPublicConstructorFound()
        {
            var error = Assert.Throws<ReduxException>(() => new ComplexTypeReducer<NoPublicConstructors>());

            Assert.Equal(InvalidCtor(typeof(NoPublicConstructors)), error.Message);
        }

        [Fact]
        public void ThrowsIfTooManyPublicConstructorsFound()
        {
            var error = Assert.Throws<ReduxException>(() => new ComplexTypeReducer<TooManyPublicConstructors>());

            Assert.Equal(InvalidCtor(typeof(TooManyPublicConstructors)), error.Message);
        }

        [Fact]
        public void ThrowsIfAnyConstructorParametersDoNotMapToPublicProperties()
        {
            var error = Assert.Throws<ReduxException>(() => new ComplexTypeReducer<ConstructorParameterMismatch>());

            Assert.Equal(InvalidCtorArg(typeof(ConstructorParameterMismatch), "name"), error.Message);
        }
    }
}
