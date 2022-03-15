using System;
using System.Collections.Generic;
using System.Numerics;

namespace Symbolica.Computation;

internal interface IConstraints : IDisposable
{
    void Assert(IEnumerable<IValue> assertions);
    bool IsSatisfiable(IValue assertion);
    BigInteger GetSingleValue(IValue value);
    BigInteger GetExampleValue(IValue value);
    IEnumerable<KeyValuePair<string, string>> GetExampleValues();
}
