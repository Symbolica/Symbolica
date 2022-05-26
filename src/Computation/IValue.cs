using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IValue : IEquatable<IValue>, IMergeable<(IValue, IValue), IValue>
{
    Bits Size { get; }
    ISet<IValue> Symbols { get; }

    BitVecExpr AsBitVector(ISolver solver);
    BoolExpr AsBool(ISolver solver);
    FPExpr AsFloat(ISolver solver);
    bool TryMerge(IValue value, out IValue? merged);
    IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs);
}
