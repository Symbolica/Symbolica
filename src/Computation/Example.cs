﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Example : IExample
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
