using System.Collections;
using System.Collections.Generic;

namespace Transmute
{
    public class TestActions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {TestAction1.Value};
            yield return new object[] {TestAction2.Value};
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}