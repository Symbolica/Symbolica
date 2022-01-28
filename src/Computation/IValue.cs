using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IValue
{
    Bits Size { get; }
    IEnumerable<IValue> Children { get; }
    string? PrintedValue { get; }

    BigInteger AsConstant(IContext context);
    BitVecExpr AsBitVector(IContext context);
    BoolExpr AsBool(IContext context);
    FPExpr AsFloat(IContext context);
}
