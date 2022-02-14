using System;
using System.Collections.Generic;
using System.Numerics;

namespace Symbolica.Computation;

internal interface IConstraints : IDisposable
{
    bool IsSatisfiable(IValue assertion);
    BigInteger Evaluate(IValue value);
    IEnumerable<KeyValuePair<string, string>> Evaluate();
}
