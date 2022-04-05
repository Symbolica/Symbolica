using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IValue : IEquatable<IValue>
{
    Size Size { get; }

    BitVecExpr AsBitVector(ISolver solver);
    BoolExpr AsBool(ISolver solver);
    FPExpr AsFloat(ISolver solver);
}
