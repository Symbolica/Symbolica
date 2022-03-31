using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IValue : IEquatable<IValue>
{
    Bits Size { get; }
    IEnumerable<IValue> Children { get; }
    string? PrintedValue { get; }

    BitVecExpr AsBitVector(ISolver solver);
    BoolExpr AsBool(ISolver solver);
    FPExpr AsFloat(ISolver solver);
    IValue BitCast(Bits targetSize);
    IValue ToBits();
}
