using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface ISolver : IDisposable
{
    Context Context { get; }

    void Assert(IEnumerable<IExpression> assertions);
    void Assert(string name, IEnumerable<IExpression> assertions);
    bool IsSatisfiable(IExpression assertion);
    BigInteger GetSingleValue(IExpression expression);
    BigInteger GetExampleValue(IExpression expression);
    IExample GetExample();
    bool TryGetSingleValue(IExpression expression, out BigInteger constant);
}
