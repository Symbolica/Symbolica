using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class UnsignedLess : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedLess(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c => c.MkBVULT(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Less(r.AsUnsigned())
            : new UnsignedLess(left, right);
    }
}
