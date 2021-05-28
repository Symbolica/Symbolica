using System.Collections;
using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class Example : IExample
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _pairs;

        private Example(IEnumerable<KeyValuePair<string, string>> pairs)
        {
            _pairs = pairs;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static IExample Create(IPersistentSpace space)
        {
            using var model = space.GetModel();

            return new Example(model.Evaluate());
        }
    }
}
