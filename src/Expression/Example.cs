using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Symbolica.Expression;

public sealed class Example : IEnumerable<KeyValuePair<string, string>>
{
    private readonly KeyValuePair<string, string>[] _pairs;

    public Example(KeyValuePair<string, string>[] pairs)
    {
        _pairs = pairs;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return _pairs.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
