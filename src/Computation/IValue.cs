using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IValue : IEquatable<IValue>
{
    Bits Size { get; }

    BitVecExpr AsBitVector(ISolver solver);
    BoolExpr AsBool(ISolver solver);
    FPExpr AsFloat(ISolver solver);
    bool TryMerge(IValue value, [MaybeNullWhen(false)] out IValue merged);
}
