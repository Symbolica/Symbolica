using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface ISolver : IDisposable
{
    Context Context { get; }

    void Assert(IEnumerable<IValue> assertions);
    void Assert(string name, IEnumerable<IValue> assertions);
    bool IsSatisfiable(IValue assertion);
    BigInteger? TryGetSingleValue(IValue value);
    BigInteger GetExampleValue(IValue value);
    IExample GetExample();
}
