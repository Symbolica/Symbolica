using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class FloatDivide : Float
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatDivide(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override FPExpr AsFloat(IContext context)
    {
        using var rm = context.CreateExpr(c => c.MkFPRNE());
        using var t1 = _left.AsFloat(context);
        using var t2 = _right.AsFloat(context);
        return context.CreateExpr(c => c.MkFPDiv(rm, t1, t2));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(l / r),
            (l, r) => new ConstantDouble(l / r),
            (l, r) => new FloatDivide(l, r));
    }
}
