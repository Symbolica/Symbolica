using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class FloatLess : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatLess(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c => c.MkFPLt(_left.AsFloat(context), _right.AsFloat(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Float.Binary(left, right,
            (l, r) => new ConstantBool(l < r),
            (l, r) => new ConstantBool(l < r),
            (l, r) => new FloatLess(l, r));
    }
}
