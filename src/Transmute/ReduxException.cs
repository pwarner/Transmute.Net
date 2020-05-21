using System;

namespace Transmute
{
    public class ReduxException : Exception
    {
        internal ReduxException(string message) : base(message)
        {
        }

        public static string InvalidCtor(Type stateType) =>
            $"A complex type used to represent state '{stateType.FullName}' must have a single, public constructor with 1 or more parameters.";

        public static string InvalidCtorArg(Type stateType, string parameter) =>
            $"The constructor for complex type used to represent state '{stateType.FullName}' " +
            $"has a parameter '{parameter}' that does not match one of its public properties.";
    }
}