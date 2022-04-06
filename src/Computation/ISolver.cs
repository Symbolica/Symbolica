using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface ISolver : IDisposable
{
    Context Context { get; }

    void Assert(IEnumerable<IExpression<IType>> assertions);
    void Assert(string name, IEnumerable<IExpression<IType>> assertions);
    bool IsSatisfiable(IExpression<IType> assertion);
    BigInteger GetSingleValue(IExpression<IType> expression);
    BigInteger GetExampleValue(IExpression<IType> expression);
    Example GetExample();
}
