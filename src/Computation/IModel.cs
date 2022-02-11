using System;
using System.Collections.Generic;
using System.Numerics;

namespace Symbolica.Computation;

internal interface IModel : IDisposable
{
    bool IsSatisfiable(IValue assertion);
    BigInteger Evaluate(IValue value);
    IEnumerable<KeyValuePair<string, string>> Evaluate();
}
